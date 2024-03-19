using EastAsianWidthDotNet;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using PlayerControl.Model;
using PlayerControl.View;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Servicies;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlayerControl.ViewModels
{
	public class MainViewModel : BindableViewModel
	{
		private const String _htmlSourceFile = "scoreboard.html";
		private const String _jsonFileName = "streamcontrol.json";
		private const String _backupJsonFileName = "LastOperationBackup.json";

		// CurrentPlayerをクリアするための空PlayerModel インスタンス
		private PlayerModel _emptyPlayer { get; } = new PlayerModel();

		private SemaphoreSlim _semaphoreSlimSelialize = new(1, 1);


		// ユーザー入力データ Model のインスタンス（バックアップ対象）▶コンストラクタで初期化
		public OperationModel _operation { get; }

		// app.config から取得した設定値
		public int _notepadTabCount { get; } = 8;
		public String _defaultCountry { get; } = "blk";

		#region Viewと連携するUI用プロパティ
		public IDialogCoordinator PlayerControlDialogCoordinator { get; set; } = new DialogCoordinator();
		public ReactivePropertySlim<SnackbarMessageQueue> PlayerEditSnackbarMessageQueue { get; } = new ReactivePropertySlim<SnackbarMessageQueue>(new SnackbarMessageQueue());
		public CollectionViewSource PlayersViewSource { get; set; } = new CollectionViewSource();
		public ReactivePropertySlim<String> OutputJsonPath { get; } = new ReactivePropertySlim<String>();
		public ReactivePropertySlim<DateTime> OutputJsonTime { get; } = new ReactivePropertySlim<DateTime>(DateTime.MinValue);
		public ReactivePropertySlim<DateTime> SetClipboardTime { get; } = new ReactivePropertySlim<DateTime>(DateTime.MinValue);
		public ReactivePropertySlim<PlayerModel> SelectedPlayer { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrentPlayer1 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrentPlayer2 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<String> AppTitle { get; } = new ReactivePropertySlim<String>("PlayerControl");
		public ReactivePropertySlim<String> DefaultCountry { get; } = new ReactivePropertySlim<String>();
		// Single/Mixture 対応(Ver0.3.5)
		public ReactivePropertySlim<ScoreMode> CurrentScoreMode { get; } = new ReactivePropertySlim<ScoreMode>(ScoreMode.Single);

		#endregion

		#region バックアップ対象となる Property (_operation モデルと連携する)
		public ReactiveProperty<String> Stage { get; }
		// HTMLのスコアの上に表示する文字列
		public ReactiveProperty<String> ScoreLabel { get; }

		// PlayerControl の スコア入力枠に表示するラベル 複数スコア入力モードでのみ仕様（HTMLには影響しない）
		public ReactivePropertySlim<String> UI_Score1Label { get; } = new ReactivePropertySlim<String>("NOR");
		public ReactivePropertySlim<String> UI_Score2Label { get; } = new ReactivePropertySlim<String>("20G");

		// ※ ReadOnlyReactivePropertyを使うと、gong-wpf-dragdrop を使った順番操作時に例外が発生する（ReadOnly非対応）
		// OperationModel の _players プロパティのインスタンスを直接参照する
		// _players()はPrivateなので、参照・追加・削除などのアクセスはOperationModelのメソッドを介して行う
		public ObservableCollection<PlayerModel> Players { get => _operation.GetPlayers(); }
		#endregion

		#region ReactiveCommand
		public ReactiveCommand Player1ChangeCommand { get; }
		public ReactiveCommand Player2ChangeCommand { get; }
		public ReactiveCommand ScoreSettingCommand { get; }
		public ReactiveCommand AboutBoxCommand { get; }
		public ReactiveCommand LoadedCommand { get; }
		public ReactiveCommand ClosingCommand { get; }
		public ReactiveCommand EditPlayersListCommand { get; }
		public ReactiveCommand RemovePlayerCommand { get; }
		public ReactiveCommand<Object> AddPlayerCommand { get; }
		public ReactiveCommand PlayerExchangeCommand { get; }
		public ReactiveCommand<string> PlayerClearCommand { get; }
		public ReactiveCommand SaveJsonCommand { get; }
		public ReactiveCommand SetStageCommand { get; }
		public ReactiveCommand ClearStageCommand { get; }
		public ReactiveCommand ClearScoreLabelCommand { get; }
		public ReactiveCommand ClearAllPlayersScoreCommand { get; }
		public ReactiveCommand ShufflePlayersCommand { get; }
		public ReactiveCommand ListItemToClipboardCommand { get; }
		public ReactiveCommand SelectedPlayerToClipboardCommand { get; }
		public ReactiveCommand CurrentPlayersToClipboardCommand { get; }
		public ReactiveCommand AllPlayersToClipboardCommand { get; }
		public ReactiveCommand OpenOutputFolderCommand { get; }
		public ReactiveCommand CloseModalWindowCommand { get; }
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			// app.config の値を格納
			var param = ConfigurationManager.AppSettings["NotePadTabCount"];
			if (int.TryParse(param, out var tabCount))
			{
				_notepadTabCount = tabCount;
			}

			var paramDC = ConfigurationManager.AppSettings["DefaultCountry"];
			if (!String.IsNullOrEmpty(paramDC))
			{
				DefaultCountry.Value = paramDC;
			}

			// OperationModelを初期化
			OperationModel? restore = RestoreBackupPlayersData();   // バックアップファイルがある場合
			if (restore != null)
			{
				_operation = restore;
			}
			else
			{
				_operation = new OperationModel();
			}

			// Model-VM 連携（※ Players プロパティはOperationModelの_playerを直接参照 詳細は Players プロパティの定義部に記載）
			this.Stage = _operation.ToReactivePropertyAsSynchronized(m => m.Stage).AddTo(Disposable);
			this.ScoreLabel = _operation.ToReactivePropertyAsSynchronized(m => m.ScoreLabel).AddTo(Disposable);

			// アプリタイトルを設定
			AssemblyAttribute assmAttr = new();
			Assembly asm = Assembly.GetExecutingAssembly();
			try
			{
				if (asm != null)
				{
					AssemblyName asmName = asm.GetName();
					if (asmName != null)
					{
						var AppName = asmName.Name;

						// 属性から製品名が取得できたらそちらを使う
						var productarray = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
						if (productarray != null && productarray.Length > 0)
						{
							AppName = ((AssemblyProductAttribute)productarray[0]).Product;
						}
						AppTitle.Value = $"{AppName} ver {asmName.Version}";
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[GetScoreText]Exception {ex.ToString()}");
				// アセンブリ情報（タイトルバー表示）が取れない場合はなにもせずデフォルト名を表示する
			}

			// ReactiveProperty更新時の処理
			CurrentScoreMode.Subscribe(x =>
			{
				SaveStreamControlJson();
			});

			// アプリ開始コマンド
			LoadedCommand = new ReactiveCommand();
			LoadedCommand.Subscribe(_ =>
			{
				// アプリ起動時の初期化処理
				Initialize();

				// 現在のVM情報からStreamControl Jsonファイルを保存
				SaveStreamControlJson();

				// スコアボード用ViewSourceを設定
				PlayersViewSource.Source = Players;
				PlayersViewSource.SortDescriptions.Clear();
				PlayersViewSource.SortDescriptions.Add(new SortDescription("Score", ListSortDirection.Descending));
				PlayersViewSource.IsLiveSortingRequested = true;    // 自動ソートフラグ
			}).AddTo(Disposable);

			// アプリ終了前コマンド
			ClosingCommand = new ReactiveCommand();
			ClosingCommand.Subscribe(_ =>
			{
				try
				{
					// StreamControl Jsonファイルを初期化（タイムスタンプも初期化）
					CurrentPlayer1.Value = _emptyPlayer;
					CurrentPlayer2.Value = _emptyPlayer;
					_operation.Clear();
					SaveStreamControlJson(true);

					// バックアップファイルも消去
					var backupFile = GetBackupFilePath();
					if (File.Exists(backupFile))
					{
						File.Delete(backupFile);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"ClosingCommand Exception:{ex.ToString()}");
				}
			}).AddTo(Disposable);

			// プレイヤー編集コマンド
			EditPlayersListCommand = new ReactiveCommand();
			EditPlayersListCommand.Subscribe(_ =>
			{
				EditPlayersList();
			}).AddTo(Disposable);

			// スコア・サブ情報（twitterなど）編集コマンド　
			ScoreSettingCommand = new ReactiveCommand();
			ScoreSettingCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe && fe.DataContext is PlayerModel player)
				{
					PlayerScoreSetting(player);
				}
			}).AddTo(Disposable);

			// プレイヤー1切り替えコマンド
			Player1ChangeCommand = new ReactiveCommand();
			Player1ChangeCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe && fe.DataContext is PlayerModel player)
				{
					ChangePlayer(player, true);
				}
			}).AddTo(Disposable);

			// プレイヤー2切り替えコマンド
			Player2ChangeCommand = new ReactiveCommand();
			Player2ChangeCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe && fe.DataContext is PlayerModel player)
				{
					ChangePlayer(player, false);
				}
			}).AddTo(Disposable);

			// プレイヤー削除コマンド
			RemovePlayerCommand = new ReactiveCommand();
			RemovePlayerCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe &&
					fe.DataContext is PlayerModel player)
				{
					_operation.RemovePlayer(player);
				}
			}).AddTo(Disposable);

			// プレイヤー入れ替えコマンド
			PlayerExchangeCommand = new ReactiveCommand();
			PlayerExchangeCommand.Subscribe(x =>
			{
				PlayerModel temp = CurrentPlayer1.Value;
				CurrentPlayer1.Value = CurrentPlayer2.Value;
				CurrentPlayer2.Value = temp;
				SaveStreamControlJson();
			}).AddTo(Disposable);

			// プレイヤークリアコマンド
			PlayerClearCommand = new ReactiveCommand<string>();
			PlayerClearCommand.Subscribe(x =>
			{
				if (x == "Player1" || x == "PlayerAll")
				{
					CurrentPlayer1.Value = _emptyPlayer;
				}
				if (x == "Player2" || x == "PlayerAll")
				{
					CurrentPlayer2.Value = _emptyPlayer;
				}
				SaveStreamControlJson();
			}).AddTo(Disposable);

			// プレイヤー追加コマンド
			AddPlayerCommand = new ReactiveCommand<Object>();
			AddPlayerCommand.Subscribe(param =>
			{
				if (param is TextBox textBox)
				{
					var name = textBox.Text;
					var trimName = name.Trim();

					// サブ情報は空の状態で追加する（再編集画面で設定）
					var trimTwitter = String.Empty;

					if (!String.IsNullOrEmpty(trimName))
					{
						// 同じ名前のユーザーがいないかチェック
						PlayerModel? sameNamePlayer = SearchPlayer(trimName);

						// 同じ名前がすでに存在した場合
						if (sameNamePlayer != null)
						{
							// 警告を出す
							PlayerEditSnackbarMessageQueue.Value.Enqueue($"[{trimName}] 同じ名前が登録されています");
						}
						// ユーザーを追加
						else
						{
							_operation.AddPlayer(new PlayerModel(trimName, trimTwitter, 0, 0, _defaultCountry));
							SaveStreamControlJson();
							textBox.Text = String.Empty;
						}
					}
				}
			}).AddTo(Disposable);

			// 現在の状態でJSONファイルを保存し直すコマンド
			SaveJsonCommand = new ReactiveCommand();
			SaveJsonCommand.Subscribe(_ =>
			{
				SaveStreamControlJson();
			}).AddTo(Disposable);

			// Stage文字列をJSONに保存するコマンド
			SetStageCommand = new ReactiveCommand();
			SetStageCommand.Subscribe(_ =>
			{
				SaveStreamControlJson();
			}).AddTo(Disposable);

			// Stage文字列をクリアしてJSONを更新するコマンド
			ClearStageCommand = new ReactiveCommand();
			ClearStageCommand.Subscribe(_ =>
			{
				Stage.Value = String.Empty;
				SaveStreamControlJson();
			}).AddTo(Disposable);

			// スコアラベルクリアしてJSONを更新するコマンド
			ClearScoreLabelCommand = new ReactiveCommand();
			ClearScoreLabelCommand.Subscribe(_ =>
			{
				ScoreLabel.Value = String.Empty;
				SaveStreamControlJson();
			}).AddTo(Disposable);

			// プレイヤーの並びをシャッフルするコマンド
			ShufflePlayersCommand = new ReactiveCommand();
			ShufflePlayersCommand.Subscribe(_ =>
			{
				ShufflePlayers();
			}).AddTo(Disposable);

			// 指定されたListBoxItemのユーザー名とスコアをクリップボードに貼り付けるコマンド（コンテキストメニュー用）
			ListItemToClipboardCommand = new ReactiveCommand();
			ListItemToClipboardCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe && fe.DataContext is PlayerModel player)
				{
					SetPlayerModelNameAndScoreToClipboard(player);
				}
			}).AddTo(Disposable);

			// 選択中のプレイヤーのユーザー名とスコアをクリップボードに貼り付けるコマンド
			SelectedPlayerToClipboardCommand = new ReactiveCommand();
			SelectedPlayerToClipboardCommand.Subscribe(x =>
			{
				if (SelectedPlayer.Value != null)
				{
					SetSelectedPlayerNameAndScoreToClipboard();
				}
			}).AddTo(Disposable);

			// カレントプレイヤー(1P/2P)のユーザー名とスコアをクリップボードに貼り付けるコマンド
			CurrentPlayersToClipboardCommand = new ReactiveCommand();
			CurrentPlayersToClipboardCommand.Subscribe(x =>
			{
				SetCurrentPlayerNameAndScoreToClipboard();
			}).AddTo(Disposable);

			// 全プレイヤーのユーザー名とスコアをクリップボードに貼り付けるコマンド
			AllPlayersToClipboardCommand = new ReactiveCommand();
			AllPlayersToClipboardCommand.Subscribe(x =>
			{
				SetAllPlayerNameAndScoreToClipboard();
			}).AddTo(Disposable);

			// 全プレイヤーのスコアをクリアするコマンド
			ClearAllPlayersScoreCommand = new ReactiveCommand();
			ClearAllPlayersScoreCommand.Subscribe(x =>
			{
				ClearAllPlayersScore();
			}).AddTo(Disposable);

			OpenOutputFolderCommand = new ReactiveCommand();
			OpenOutputFolderCommand.Subscribe(x =>
			{
				try
				{
					// JSON保存フォルダを開く
					var jsonPath = GetStreamControlJsonPath();
					var jsonFolder = System.IO.Path.GetDirectoryName(jsonPath);
					if (Directory.Exists(jsonFolder))
					{
						System.Diagnostics.Process.Start("EXPLORER.EXE", jsonFolder);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"OpenOutputFolderCommand Exception:{ex.ToString()}");
				}
			}).AddTo(Disposable);

			// モーダルウィンドウを閉じる
			CloseModalWindowCommand = new ReactiveCommand();
			CloseModalWindowCommand.Subscribe(x =>
			{
				if (x is System.Windows.Window wnd)
				{
					wnd.Close();
				}
			}).AddTo(Disposable);

			// このアプリについて表示する
			AboutBoxCommand = new ReactiveCommand();
			AboutBoxCommand.Subscribe(async _ =>
			{
				EnvironmentInfo _envInfo = new();

				try
				{
					// イベント発火元コントロールのDataContextからVMを取得して更新
					AboutView view = new()
					{
						DataContext = this
					};
					var result = await DialogHost.Show(view, "MainWindowDialog");
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Exception AboutBox:{ex.ToString()}");
				}

			});
		}

		/// <summary>
		/// MainWindow 初期化時にコールされる初期化処理
		/// </summary>
		public void Initialize()
		{
			// プレイヤー履歴を読み込む（ver 0.3.5 未実装）
			InitPlayersHistory();
		}

		/// <summary>
		/// JSONファイルの保存先フルパスを取得する
		/// 
		/// </summary>
		/// <returns></returns>
		private String GetStreamControlJsonPath()
		{
			// 出力先が未設定の場合
			if (!Directory.Exists(OutputJsonPath.Value))
			{
				// 実行ファイルの親フォルダを辿って scoreboard.html が存在するフォルダを探す
				var scoreboardHtmlPath = SearchStreamControlPath();

				// 見つかったらJSON保存先として設定
				if (Directory.Exists(scoreboardHtmlPath))
				{
					OutputJsonPath.Value = scoreboardHtmlPath;
				}
				// 親を辿ってみつからなかったら手動で設定させる(デバッグ中はキャンセル)
				else
				{
#if !DEBUG
					MessageBox.Show($"アプリの上位フォルダにStreamControl用 [scoreboard.html] 保存フォルダが見つかりません\n\n[scoreboard.html]が保存されているフォルダを指定してください");
					var openDlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog()
					{
						Title = "[scoreboard.html] が存在するフォルダを選択してください",
						InitialDirectory = AppContext.BaseDirectory,
						IsFolderPicker = true,	// フォルダ選択モード
					};
					if (openDlg.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
					{
						OutputJsonPath.Value = openDlg.FileName;
					}
#endif
				}
			}

			// Path + Jsonファイル名
			return System.IO.Path.Combine(OutputJsonPath.Value, _jsonFileName);
		}

		/// <summary>
		/// StreamControl互換JSONを保存する
		/// </summary>
		/// <returns></returns>
		public async void SaveStreamControlJson(bool InitTimeStump = false)
		{
			try
			{
				// Path + Jsonファイル名
				var savepath = GetStreamControlJsonPath();
				// 保存先が存在するかチェック
				if (!Directory.Exists(System.IO.Path.GetDirectoryName(savepath)))
				{
					return;
				}
				// シリアライズするStreamControlParamを作成
				StreamControlParam StreamControlData = CreateStreamControlParam(InitTimeStump);

				// ※ 短時間に連続して保存するとscoreboard.htmlが取りこぼすことがある
				// 前回保存時間から0.5秒以内なら最大1秒待機する
				await _semaphoreSlimSelialize.WaitAsync();
				try
				{
					var diff = (DateTime.Now - OutputJsonTime.Value).TotalMilliseconds;
					if (diff < 500)
					{
						Debug.WriteLine($"diff:{diff}");
						await Task.Delay(1000 - (int)diff);
					}
					using StreamWriter sw = new(savepath, false, Encoding.UTF8);
					using TextWriter writerSync = TextWriter.Synchronized(sw);
					// JSONにシリアライズして保存
					var json = JsonConvert.SerializeObject(StreamControlData);
					await writerSync.WriteAsync(json);
					await writerSync.FlushAsync();
					// 保存時間を記録
					OutputJsonTime.Value = DateTime.Now;
#if DEBUG
					var debugText = $"Flush >> [{StreamControlData.timestamp} 1P:{StreamControlData.pName1} 2P:{StreamControlData.pName2}]";
					Debug.WriteLine(debugText);
#endif
					// 保存情報をバックアップ（強制終了時の復元用）
					// BackupPlayersData();
				}
				finally
				{
					_semaphoreSlimSelialize.Release();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[SaveStreamControlJson]Exception {ex.ToString()}");
			}
		}

		/// <summary>
		/// クラスのプロパティからシリアライズするデータ（StreamControlParam）を作成する
		/// </summary>
		/// <returns></returns>
		private StreamControlParam CreateStreamControlParam(bool InitTimeStump = false)
		{
			try
			{
				StreamControlParam _scParam = new(_defaultCountry);

				// Stage名とScoreLabel文字列を正規化
				if (!String.IsNullOrEmpty(ScoreLabel.Value))
				{
					ScoreLabel.Value = ScoreLabel.Value.Trim();
				}
				if (!String.IsNullOrEmpty(Stage.Value))
				{
					Stage.Value = Stage.Value.Trim();
				}
				// Stage名をセット（改行コードを</br> に置換）
				var stagestr = Stage.Value;
				var StageLineCount = 0;   // 改行の数
				if (!String.IsNullOrEmpty(Stage.Value))
				{
					try
					{
						var temp = stagestr;
						StageLineCount = temp.Split(Environment.NewLine).Length;
						// 改行コードを</br>に置換
						stagestr = Stage.Value.Replace(Environment.NewLine, "</br>");

						// 改行が存在する場合はフォントサイズを調整
						if (StageLineCount > 1)
						{
							// 行数に応じて文字サイズを調整
							var adjustSize = 0;
							// 2行
							if (StageLineCount == 2)
							{
								adjustSize = -1;
							}
							// 3行以上
							else
							{
								adjustSize = -2;
							}
							stagestr = $"<font size={adjustSize}>" + stagestr + "</font>";
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"[Stage NewLine Replace + FontSize Adjust]Exception {ex.ToString()}");
						// 例外発生時はスルー
					}
				}
				_scParam.stage = stagestr;

				// プレイヤー１情報
				if (CurrentPlayer1.Value != null)
				{
					_scParam.pName1 = CurrentPlayer1.Value.Name;
					_scParam.pTwitter1 = CurrentPlayer1.Value.Twitter;
					_scParam.pScore1 = GetScoreText(CurrentPlayer1.Value);
					// 国文字が指定されていないはデフォルト文字列をセット
					if (!String.IsNullOrEmpty(CurrentPlayer1.Value.Country))
					{
						_scParam.pCountry1 = CurrentPlayer1.Value.Country;
					}
					else
					{
						_scParam.pCountry1 = DefaultCountry.Value;
					}
				}
				// プレイヤー２情報
				if (CurrentPlayer2.Value != null)
				{
					_scParam.pName2 = CurrentPlayer2.Value.Name;
					_scParam.pTwitter2 = CurrentPlayer2.Value.Twitter;
					_scParam.pScore2 = GetScoreText(CurrentPlayer2.Value);
					// 国文字が指定されていないはデフォルト文字列をセット
					if (!String.IsNullOrEmpty(CurrentPlayer2.Value.Country))
					{
						_scParam.pCountry2 = CurrentPlayer2.Value.Country;
					}
					else
					{
						_scParam.pCountry2 = DefaultCountry.Value;
					}
				}

				// タイムスタンプを初期化する場合
				if (InitTimeStump)
				{
					_scParam.timestamp = "0";
				}
				return _scParam;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception CreateStreamControlParam:{ex.ToString()}");
				throw;
			}
		}

		/// <summary>
		/// 指定された文字列をクリップボードに積む
		/// 空文字はなにもしない
		/// </summary>
		/// <param name="text"></param>
		private void SetTextToClipboard(String text)
		{
			try
			{
				if (!String.IsNullOrEmpty(text))
				{
					Clipboard.SetText(text);
					// 実行時刻を更新
					SetClipboardTime.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// ※SetText()を呼ぶと例外が発生することが頻繁にあるが、実際にはクリップボードには正常にコピーされている
				Debug.WriteLine($"Exception SetTextToClipboard:{ex.ToString()}");
			}
		}

		/// <summary>
		/// 指定されたPlayerModelオブジェクトのプレイヤー名とスコアをクリップボードに積む
		/// </summary>
		/// <param name="datacontext"></param>
		/// <summary>
		public void SetPlayerModelNameAndScoreToClipboard(object datacontext)
		{
			if (datacontext is PlayerModel player)
			{
				SetTextToClipboard(GetNameAndScoreText(player) + "\n");
			}
		}

		/// 指定されたPlayerModelオブジェクトのプレイヤー名とスコアをクリップボードに積む
		/// </summary>
		public void SetSelectedPlayerNameAndScoreToClipboard()
		{
			if (SelectedPlayer.Value != null)
			{
				SetTextToClipboard(GetNameAndScoreText(SelectedPlayer.Value) + "\n");
			}
		}
		/// <summary>
		/// 1P/2P カレントプレイヤー名とスコアをクリップボードに積む
		/// </summary>
		public void SetCurrentPlayerNameAndScoreToClipboard()
		{
			var clipboardText = String.Empty;

			// 名前の最大幅を取得
			var maxNameWidth = 0;
			if(CurrentPlayer1.Value != null && CurrentPlayer1.Value != _emptyPlayer)
			{
				maxNameWidth = CurrentPlayer1.Value.Name.GetWidth();
			}
			if( CurrentPlayer2.Value != null && CurrentPlayer2.Value != _emptyPlayer)
			{
				if (maxNameWidth < CurrentPlayer2.Value.Name.GetWidth())
				{
					maxNameWidth = CurrentPlayer2.Value.Name.GetWidth();
				}
			}	

			if (CurrentPlayer1.Value != null && CurrentPlayer1.Value != _emptyPlayer)
			{
				clipboardText += GetNameAndScoreText(CurrentPlayer1.Value, maxNameWidth) + "\n";
			}
			if (CurrentPlayer2.Value != null && CurrentPlayer2.Value != _emptyPlayer)
			{
				clipboardText += GetNameAndScoreText(CurrentPlayer2.Value, maxNameWidth) + "\n";
			}
			Clipboard.SetText(clipboardText);
		}

		/// <summary>
		/// リストに追加されているすべてのプレイヤーの名前とスコアをクリップボードに積む
		/// </summary>
		public void SetAllPlayerNameAndScoreToClipboard()
		{
			try
			{
				var clipboardText = String.Empty;
				var maxNameWidth = MaxPlayerNameWidth();
				foreach (PlayerModel player in Players)
				{
					clipboardText += GetNameAndScoreText(player, maxNameWidth) + "\n";
				}
				Clipboard.SetText(clipboardText);

				// 実行時刻を更新
				SetClipboardTime.Value = DateTime.Now;
			}
			catch (Exception ex)
			{
				// ※SetText()を呼ぶと例外が発生することが頻繁にあるが、実際にはクリップボードには正常にコピーされている
				Debug.WriteLine($"Exception GetNameAndScoreText:{ex.ToString()}");
			}
		}

		/// <summary>
		/// 全プレイヤーのスコアをクリア（0）する
		/// </summary>
		public void ClearAllPlayersScore()
		{
			foreach (PlayerModel player in Players)
			{
				player.Score = 0;
				player.Score_Second = 0;
			}
			SaveStreamControlJson();
		}

		/// <summary>
		/// リスト中のプレイヤー名の中で一番文字列幅が大きな名前の文字列幅を取得する
		/// </summary>
		/// <returns></returns>
		private int MaxPlayerNameWidth()
		{
			var maxwidth = 0;
			foreach (PlayerModel checkPlayer in Players)
			{
				var w = checkPlayer.Name.GetWidth();

				if (maxwidth < w)
				{
					maxwidth = w;
				}
			}
			return maxwidth;
		}

		/// <summary>
		/// プレイヤー情報（名前＋スコア）文字列を組む
		/// コピー時にクリップボードへ積む文字として使う
		/// </summary>
		/// <param name="datacontext"></param>
		public String GetNameAndScoreText(PlayerModel player, int maxWidth)
		{
			var _tabCount = _notepadTabCount;
			if (_tabCount == 0)
			{
				_tabCount = 8; // ゼロ除算回避
			}
			try
			{
				// 名前の幅を取得
				var nameWidth = player.Name.GetWidth();

#if true		// Ver 0.3.6 タブの挿入数をシンプルに（最小1個,最大3個）
				var tabPadding = 1;
				if(maxWidth < _tabCount)
				{
					tabPadding = 1;
				}
				else if (maxWidth < _tabCount * 2)
				{
					if (nameWidth < _tabCount)
					{
						tabPadding = 2;
					}
				}
				else
				{
					if (nameWidth < _tabCount)
					{
						tabPadding = 3;
					}
					else if (nameWidth < _tabCount * 2)
					{
						tabPadding = 2;
					}
				}
#else         // ちゃんと計算しようとした場合（下記の計算ではタブがずれるのでさらに調整が必要）                                                     			                                                                                                                   
				// 最大幅をタブ数の倍数(整数丸め)に設定
				maxWidth = (maxWidth/_tabCount)* _tabCount;
				var padding = (maxWidth - nameWidth);
				var tabPadding = (padding / _tabCount) + 1;
				if (nameWidth % _tabCount != 0 && nameWidth < maxWidth)
				{
					tabPadding++;
				}
				Debug.WriteLine($"Name:{player.Name} nameWidth:{nameWidth} Padding:{padding} TabPadding:{tabPadding}");
#endif
				var tabString = new StringBuilder().Insert(0, "\t", tabPadding).ToString();

				// 名前+タブ
				var clipboardText = $"{player.Name}{tabString}";

				// モード別にスコアテキストを組む
				switch (CurrentScoreMode.Value)
				{
					case ScoreMode.Mixture:
						clipboardText += $"{UI_Score1Label}:{player.Score.ToString().PadLeft(4, ' ')}  {UI_Score2Label}:{player.Score_Second.ToString().PadLeft(4, ' ')}  合計:{(player.Score + player.Score_Second).ToString().PadLeft(4, ' ')}";
						break;
					case ScoreMode.Single:
					default:
						clipboardText += player.Score.ToString();
						break;
				}

				// 文字列が空でなければ、末尾にスペースを追加して戻す
				if( !String.IsNullOrEmpty(clipboardText))
				{
					return clipboardText + " ";
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception GetNameAndScoreText:{ex.ToString()}");
			}
			return String.Empty;
		}

		/// <summary>
		/// GetNameAndScoreText()のmaxWidth省略オーバーロード
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public String GetNameAndScoreText(PlayerModel player)
		{
			if (player != null)
			{
				return GetNameAndScoreText(player, player.Name.GetWidth());
			}
			return String.Empty;
		}

		/// <summary>
		/// プレイヤー一覧設定ウィンドウを表示する
		/// </summary>
		public void EditPlayersList()
		{
			PlayersEditWindow playerEditWindow = new();
			playerEditWindow.DataContext = this;
			playerEditWindow.ShowDialog();
		}

		/// <summary>
		/// 指定されたプレイヤーを1P or 2P に設定する
		/// </summary>
		/// <param name="datacontext"></param>
		/// <param name="isPlayer1"></param>
		public void ChangePlayer(object datacontext, bool isPlayer1)
		{
			if (datacontext is PlayerModel player)
			{
				if (isPlayer1)
				{
					CurrentPlayer1.Value = player;
					if (CurrentPlayer2.Value == player)
					{
						CurrentPlayer2.Value = _emptyPlayer;
					}
				}
				else
				{
					CurrentPlayer2.Value = player;
					if (CurrentPlayer1.Value == player)
					{
						CurrentPlayer1.Value = _emptyPlayer;
					}
				}
				SaveStreamControlJson();
			}

		}

		/// <summary>
		/// スコア入力コントロールを表示する（サブ情報も編集可能にする）
		/// ※ MainWindow専用
		/// </summary>
		/// <param name="datacontext"></param>
		async public void PlayerScoreSetting(object datacontext)
		{
			try
			{
				// イベント発火元コントロールのDataContextからVMを取得して更新
				if (datacontext is PlayerModel player)
				{
					// 下記行を入れないとアニメーション中にキー操作すると元ダイアログにフォーカスが移動してしまう
					System.Windows.Input.Keyboard.ClearFocus();

					// スコア設定Viewを表示(モードによってラベルを設定)
					var label1 = ScoreLabel.Value;
					var label2 = String.Empty;
					if (CurrentScoreMode.Value == ScoreMode.Mixture)
					{
						label1 = UI_Score1Label.Value;
						label2 = UI_Score2Label.Value;
					}
					ScoreSettingView view = new(CurrentScoreMode.Value, label1, label2)
					{
						DataContext = player
					};
					var result = await DialogHost.Show(view, "MainWindowDialog");
					if (result is bool dlgResult && dlgResult)
					{
						// 現在選択中のユーザーのスコアを更新した場合
						if (CurrentPlayer1.Value == player || CurrentPlayer2.Value == player)
						{
							SaveStreamControlJson();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception ScoreImput:{ex.ToString()}");
			}
		}

		/// <summary>
		/// プレイヤーサブ情報(twitter等)を入力
		/// ※ MainWindow専用
		/// </summary>
		/// <param name="datacontext"></param>

		/// <summary>
		/// プレイヤー名ダイアログでプレイヤー名を入力する
		/// ※ PlayersEdit専用
		/// </summary>
		/// <param name="datacontext"></param>
		async public void InputPlayerName(object datacontext)
		{
			try
			{
				// イベント発火元コントロールのDataContextからVMを取得して更新
				if (datacontext is PlayerModel player)
				{
					PlayerSettingView view = new()
					{
						EditName = player.Name,
						EditTwitter = player.Twitter,
						EditCountry = player.Country
					};
					// 国名が空のときはデフォルトをセット
					if (String.IsNullOrEmpty(player.Country))
					{
						view.EditCountry = DefaultCountry.Value;
					}


					var result = await DialogHost.Show(view, "PlayersEditDialog");
					if (result is bool dlgResult && dlgResult)
					{
						var trimName = view.EditName;

						// 同じ名前のユーザーがいないかチェック
						PlayerModel? sameNamePlayer = SearchPlayer(trimName);

						// 自分自身以外で!、同じ名前が存在した場合
						if (sameNamePlayer != null && sameNamePlayer != player)
						{
							// 警告を出す
							PlayerEditSnackbarMessageQueue.Value.Enqueue($"[{trimName}]同じ名前が登録されています");
						}
						// ユーザー名を更新
						else
						{
							player.Name = trimName;
						}
						// twitterを更新（特にチェックなし）
						player.Twitter = view.EditTwitter;
						// countryを更新
						if (!String.IsNullOrEmpty(view.EditCountry))
						{
							player.Country = view.EditCountry;
						}
						else
						{
							player.Country = DefaultCountry.Value;
						}
						// JSON更新
						SaveStreamControlJson();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception ScoreImput:{ex.ToString()}");
			}
		}

		/// <summary>
		/// プレイヤーリストの中から同じ名前のプレイヤーを探す
		/// </summary>
		/// <param name="targetName"></param>
		/// <returns></returns>
		PlayerModel? SearchPlayer(String targetName)
		{
			foreach (PlayerModel checkPlayer in Players)
			{
				if (checkPlayer.Name == targetName)
				{
					return checkPlayer;
				}
			}
			return null;
		}

		/// <summary>
		/// プレイヤーリストやステージ名などをバックアップする
		/// OperationModel 内のプロパティをJSONに保存する
		/// </summary>
		private void BackupPlayersData()
		{
			try
			{
				var jsonStr = JsonConvert.SerializeObject(_operation);

				var savepath = GetBackupFilePath();
				if (!String.IsNullOrEmpty(savepath))
				{
					using StreamWriter sw = new(savepath, false, Encoding.UTF8);
					sw.Write(jsonStr);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BackupPlayersData]Exception {ex.ToString()}");
			}
		}
		private OperationModel? RestoreBackupPlayersData()
		{
			try
			{
				var loadpath = GetBackupFilePath();
				if (!String.IsNullOrEmpty(loadpath) && File.Exists(loadpath))
				{
					var jsonStr = File.ReadAllText(loadpath);
					OperationModel? deserializedObjects = JsonConvert.DeserializeObject<OperationModel>(jsonStr);
					if (deserializedObjects != null)
					{
						return deserializedObjects;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[RestoreBackupPlayersData]Exception {ex.ToString()}");
			}
			return null;
		}

		/// <summary>
		/// ユーザー操作情報をバックアップするファイルのフルパスを取得する
		/// </summary>
		/// <returns>バックアップファイルパス</returns>
		private String? GetBackupFilePath()
		{
			// 実行ファイルのパス + Jsonファイル名 ※ .NET5以降では Assenmbly.Locationは空を返すので使用しないこと
			return System.IO.Path.Combine(AppContext.BaseDirectory, _backupJsonFileName);
		}

		/// <summary>
		/// ソースHTMLファイルが存在するパスを取得
		/// 実行ファイルの親フォルダをたどる
		/// </summary>
		/// <returns></returns>
		private String SearchStreamControlPath()
		{
			// 実行ファイルのパスを取得 ※ .NET5以降では Assenmbly.Locationは空を返すので使用しないこと
			DirectoryInfo? di = new(AppContext.BaseDirectory);
			while (true)
			{
				if (di == null)
				{
					break;
				}
				DirectoryInfo? parent = di.Parent;
				if (parent == null)
				{
					break;
				}

				var checkPath = Path.Combine(parent.FullName, _htmlSourceFile);

				if (System.IO.File.Exists(checkPath))
				{
					// ソースHTMLが存在するパスがみつかった
					return parent.FullName;
				}
				// 親をたどる
				di = di.Parent;
			}
			return String.Empty;
		}

		/// <summary>
		/// スコア値（int）からJSONに保存する文字列を生成する
		/// </summary>
		/// <param name="score"></param>
		/// <returns></returns>
		private String GetScoreText(PlayerModel player)
		{
			if (player == null || String.IsNullOrEmpty(player.Name))
			{
				return String.Empty;
			}

			// スコア値を計算（Mixtureモードなら２つのスコアの合計）
			var score = player.Score;
			if (CurrentScoreMode.Value == ScoreMode.Mixture)
			{
				score += player.Score_Second;
			}
			var scoreStr = String.Empty;
			var labelStr = String.Empty;

			try
			{
				var scoreLabelTrim = ScoreLabel.Value.Trim();

				// スコアラベルが存在する場合はサイズを調整
				if (!String.IsNullOrEmpty(scoreLabelTrim))
				{
					labelStr = $"<font size=-1 color=\"white\">{scoreLabelTrim}</font></br>";
				}
				// 桁数に合わせてスコアのフォントサイズ調整
				// 標準"5" 4桁以上は"4"
				var scoreSize = 5;
				if (score > 9999 || !String.IsNullOrEmpty(labelStr))
				{
					scoreSize--;
				}
				scoreStr = $"{labelStr}<font size={scoreSize}>{score.ToString()}</font>";
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[GetScoreText]Exception {ex.ToString()}");
				// 例外発生時は空文字を返す
			}
			return scoreStr;
		}

		/// <summary>
		/// プレイヤーの順番をシャッフルする
		/// </summary>
		private void ShufflePlayers()
		{
			if (Players.Count > 1)
			{
				// コレクションをシャッフルする（念の為ユーザー数分繰り返す）
				var shuffle = Players.ToArray();
				for (var cnt = 0; cnt < Players.Count; cnt++)
				{
					// ListアイテムそれぞれにGUID(128ビット値）を割り振ってソート
					shuffle = shuffle.OrderBy(i => Guid.NewGuid()).ToArray();
				}

				Players.Clear();
				foreach (PlayerModel? p in shuffle)
				{
					Players.Add(p);
				}
			}
		}

		/// <summary>
		/// プレイヤー履歴リストを初期化する
		/// TODO: 直前のリストを復元 or CSVから読込み or 履歴から選択
		/// </summary>
		public void InitPlayersHistory()
		{
#if DEBUG // デバッグ用ダミーデータ
#if true
			Players.Add(new PlayerModel("あいうえ", $"", 250, 500, _defaultCountry));
			Players.Add(new PlayerModel("かきく", $"", 250, 500, _defaultCountry));
			Players.Add(new PlayerModel("かきくけこさし", $"", 777, 200, _defaultCountry));
			Players.Add(new PlayerModel("たちつてとなに(G)", $"", 888, 200, _defaultCountry));
			Players.Add(new PlayerModel("G12345678", $"", 999, 200, _defaultCountry));
			Players.Add(new PlayerModel("H123456789", $"", 1010, 200, _defaultCountry));
			Players.Add(new PlayerModel("J1234567890", $"", 1111, 200, _defaultCountry));
			Players.Add(new PlayerModel("K12345678901", $"", 1212, 200, _defaultCountry));
			Players.Add(new PlayerModel("L1234567890123", $"", 1414, 200, _defaultCountry));
			Players.Add(new PlayerModel("M12345678901234", $"", 1515, 200, _defaultCountry));
			Players.Add(new PlayerModel("N123456789012345", $"", 16, 200, _defaultCountry));
#endif
#endif
		}
	}
}
