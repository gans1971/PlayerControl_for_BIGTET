using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using PlayerControl.Model;
using PlayerControl.View;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Servicies;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlayerControl.ViewModels
{
	public class MainViewModel : BindableViewModel
	{
		private const String _htmlSourceFile = "scoreboard.html";
		private const String _jsonFileName = "streamcontrol.json";

		// CurrentPlayerをクリアするためのインスタンス
		private PlayerModel _emptyPlayer { get; } = new PlayerModel();

		public IDialogCoordinator PlayerControlDialogCoordinator { get; set; } = new DialogCoordinator();

		#region ReactiveProperty
		public ReactivePropertySlim<String> AppTitle { get; } = new ReactivePropertySlim<String>("Player Control for BIGTET");
		public ReactivePropertySlim<String> OutputJsonPath { get; } = new ReactivePropertySlim<String>();

		public ReactivePropertySlim<PlayerModel> SelectedPlayer { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrentPlayer1 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrentPlayer2 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<String> Stage { get; } = new ReactivePropertySlim<String>(String.Empty);
		public ReactivePropertySlim<SnackbarMessageQueue> PlayerEditSnackbarMessageQueue { get; } = new ReactivePropertySlim<SnackbarMessageQueue>(new SnackbarMessageQueue());
		public ReactivePropertySlim<String> DefaultCountry { get; } = new ReactivePropertySlim<String>("blk");

		public ReactiveCollection<PlayerModel> Players { get; } = new ReactiveCollection<PlayerModel>();
		public ReactiveCollection<PlayerModel> PlayersHistory { get; } = new ReactiveCollection<PlayerModel>();

		public CollectionViewSource PlayersViewSource { get; set; } = new CollectionViewSource();

		#endregion

		#region ReactiveCommand
		public ReactiveCommand Player1ChangeCommand { get; }
		public ReactiveCommand Player2ChangeCommand { get; }
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
		public ReactiveCommand ToClipboardCommand { get; }
		public ReactiveCommand CloseModalWindowCommand { get; }
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			// アプリタイトルを設定
			var assmAttr = new AssemblyAttribute();
			var asm = Assembly.GetExecutingAssembly();
			try
			{
				if (asm != null)
				{
					var asmName = asm.GetName();
					if (asmName != null)
					{
						var AppName = asmName.Name;

						// 属性から製品名が取得できたらそちらを使う
						object[] productarray = asm.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
						if (productarray != null && productarray.Length > 0)
						{
							AppName = ((AssemblyProductAttribute)productarray[0]).Product;
						}
						AppTitle.Value = $"{AppName} ver {asmName.Version}";
					}
				}
			}
			catch
			{
				// アセンブリ情報（タイトルバー表示）が取れない場合はなにもせずデフォルト名を表示する
			}


			// アプリ開始コマンド
			LoadedCommand = new ReactiveCommand();
			LoadedCommand.Subscribe(_ =>
			{
				// 初期状態でStreamControl Jsonファイルを保存
				SaveStreamControlJson();

				// プレイヤー履歴を初期化
				InitPlayersHistory();

				// スコアボード用ViewSourceを設定
				PlayersViewSource.Source = Players;
				PlayersViewSource.SortDescriptions.Clear();
				PlayersViewSource.SortDescriptions.Add(new SortDescription("Score.Value", ListSortDirection.Descending));
				PlayersViewSource.IsLiveSortingRequested = true;	// 自動ソートフラグ
			}).AddTo(Disposable);

			// アプリ終了前コマンド
			ClosingCommand = new ReactiveCommand();
			ClosingCommand.Subscribe(_ =>
			{
				// StreamControl Jsonファイルを初期化（タイムスタンプも初期化）
				CurrentPlayer1.Value = _emptyPlayer;
				CurrentPlayer2.Value = _emptyPlayer;
				Stage.Value = String.Empty;
				SaveStreamControlJson(true);

			}).AddTo(Disposable);

			// プレイヤー編集コマンド
			EditPlayersListCommand = new ReactiveCommand();
			EditPlayersListCommand.Subscribe(_ =>
			{
				EditPlayersList();
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
					Players.Remove(player);
				}
			}).AddTo(Disposable);

			// プレイヤー入れ替えコマンド
			PlayerExchangeCommand = new ReactiveCommand();
			PlayerExchangeCommand.Subscribe(x =>
			{
				var temp = CurrentPlayer1.Value;
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
					if (!String.IsNullOrEmpty(trimName))
					{
						// 同じ名前のユーザーがいないかチェック
						var sameNamePlayer = SearchPlayer(trimName);

						// 同じ名前がすでに存在した場合
						if (sameNamePlayer != null)
						{
							// 警告を出す
							PlayerEditSnackbarMessageQueue.Value.Enqueue($"[{trimName}] 同じ名前が登録されています");
						}
						// ユーザーを追加
						else
						{
							Players.Add(new PlayerModel(trimName, 0, 0));
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

			// クリップボードにユーザー名とスコアを貼り付けるコマンド
			ToClipboardCommand = new ReactiveCommand();
			ToClipboardCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe && fe.DataContext is PlayerModel player)
				{
					SetPlayerInfoToClipboard(player);
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
				var _envInfo = new Servicies.EnvironmentInfo();

				try
				{
					// イベント発火元コントロールのDataContextからVMを取得して更新
					var view = new AboutView()
					{
						DataContext = this
					};
					var result = await DialogHost.Show(view, "PlayerControlWindowDialog");
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Exception AboutBox:{ex.ToString()}");
				}

			});
		}

		/// <summary>
		/// プレイヤー情報（名前＋スコア）をクリップボードに積む
		/// </summary>
		/// <param name="datacontext"></param>
		public void SetPlayerInfoToClipboard(object datacontext)
		{
			if (datacontext is PlayerModel player)
			{
				Clipboard.SetText($"{player.Name}\t{player.Score}");
			}
		}

		/// <summary>
		/// プレイヤー一覧設定ウィンドウを表示する
		/// </summary>
		public void EditPlayersList()
		{
			var playerEditWindow = new PlayersEditWindow();
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
		/// スコア入力コントロールを表示する
		/// </summary>
		/// <param name="datacontext"></param>
		async public void InputScore(object datacontext)
		{
			try
			{
				// イベント発火元コントロールのDataContextからVMを取得して更新
				if (datacontext is PlayerModel player)
				{
					// 下記行を入れないとアニメーション中にキー操作すると元ダイアログにフォーカスが移動してしまう
					System.Windows.Input.Keyboard.ClearFocus();

					var view = new ScoreSettingView()
					{
						DataContext = player
					};
					var result = await DialogHost.Show(view, "PlayerControlWindowDialog");
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
		/// プレイヤー名ダイアログでプレイヤー名を入力する
		/// </summary>
		/// <param name="datacontext"></param>
		async public void InputPlayerName(object datacontext)
		{
			try
			{
				// イベント発火元コントロールのDataContextからVMを取得して更新
				if (datacontext is PlayerModel player)
				{
					var view = new PlayerSettingView()
					{
						EditName = player.Name.Value
					};

					var result = await DialogHost.Show(view, "PlayersEditDialog");
					if (result is bool dlgResult && dlgResult)
					{
						var trimName = view.EditName;

						// 同じ名前のユーザーがいないかチェック
						var sameNamePlayer = SearchPlayer(trimName);

						// 同じ名前がすでに存在した場合
						if (sameNamePlayer != null)
						{
							// 警告を出す
							PlayerEditSnackbarMessageQueue.Value.Enqueue($"[{trimName}]同じ名前が登録されています");
						}
						// ユーザー名を更新
						else
						{
							player.Name.Value = trimName;
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
		/// プレイヤーリストの中から同じ名前のプレイヤーを探す
		/// </summary>
		/// <param name="targetName"></param>
		/// <returns></returns>
		PlayerModel? SearchPlayer(String targetName)
		{
			foreach (var checkPlayer in Players)
			{
				if (checkPlayer.Name.Value == targetName)
				{
					return checkPlayer;
				}
			}
			return null;
		}

		/// <summary>
		/// PlayerControlWindow 初期化時にコールされる初期化処理
		/// </summary>
		public void Initialize()
		{
			// プレイヤーリストを読み込む
			InitPlayersHistory();
		}

		/// <summary>
		/// ソースHTMLファイルが存在するパスを取得
		/// 実行ファイルの親フォルダをたどる
		/// </summary>
		/// <returns></returns>
		private String GetStreamControlPath()
		{
			// 実行ファイルのパスを取得 ※ .NET5以降では Assenmbly.Locationは空を返すので使用しないこと
			var di = new DirectoryInfo(AppContext.BaseDirectory);
			while (true)
			{
				if (di == null)
				{
					break;
				}
				var parent = di.Parent;
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
		/// StreamControl互換JSONを保存する
		/// </summary>
		/// <returns></returns>
		public bool SaveStreamControlJson(bool InitTimeStump = false)
		{
			try
			{
				// StreamControlのHTMLテンプレートパスを初期化
				if (!Directory.Exists(OutputJsonPath.Value))
				{
					// 実行ファイルの親フォルダを辿って scoreboard.html が存在するフォルダを探す
					var scoreboardHtmlPath = GetStreamControlPath();

					// 見つかったらJSON保存先として設定
					if (Directory.Exists(scoreboardHtmlPath))
					{
						OutputJsonPath.Value = scoreboardHtmlPath;
					}
					// 親を辿ってみつからなかったら手動で設定させる
					else
					{
						MessageBox.Show($"アプリの上位フォルダにStreamControl用 [scoreboard.html] 保存フォルダが見つかりません\n\n[scoreboard.html]が保存されているフォルダを指定してください");
						var openDlg = new CommonOpenFileDialog()
						{
							Title = "[scoreboard.html] 保存フォルダを選択してください",
							InitialDirectory = AppContext.BaseDirectory,
							// フォルダ選択モードにする
							IsFolderPicker = true,
						};
						if (openDlg.ShowDialog() != CommonFileDialogResult.Ok)
						{
							return false;
						}
						else
						{
							OutputJsonPath.Value = openDlg.FileName;
						}
					}
				}

				// 念のためチェック
				if (!Directory.Exists(OutputJsonPath.Value))
				{
					MessageBox.Show($"[streamcontrol.json] 保存先が設定できませんでした");
					return false;
				}

				// Path + Jsonファイル名
				var savepath = System.IO.Path.Combine(OutputJsonPath.Value, _jsonFileName);

				// Jsonクラスに値をセット(CurrentPlayerがnullの場合は初期値のまま保存）
				var StreamControlData = new StreamControlParam();

				// イベント名をセット
				StreamControlData.stage = Stage.Value;

				// プレイヤー１情報
				if (CurrentPlayer1.Value != null)
				{
					StreamControlData.pName1 = CurrentPlayer1.Value.Name.Value;
					StreamControlData.pScore1 = CurrentPlayer1.Value.Score.Value.ToString();
					StreamControlData.pCountry1 = DefaultCountry.Value;
				}
				// プレイヤー２情報
				if (CurrentPlayer2.Value != null)
				{
					StreamControlData.pName2 = CurrentPlayer2.Value.Name.Value;
					StreamControlData.pScore2 = CurrentPlayer2.Value.Score.Value.ToString();
					StreamControlData.pCountry2 = DefaultCountry.Value;
				}
				// タイムスタンプを初期化する場合
				if (InitTimeStump)
				{
					StreamControlData.timestamp = "0";
				}
				var json = JsonConvert.SerializeObject(StreamControlData);
				using (var sw = new StreamWriter(savepath, false, Encoding.UTF8))
				{
					sw.Write(json);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[SaveStreamControlJson]Exception {ex.ToString()}");
			}
			return false;
		}

		/// <summary>
		/// プレイヤー履歴リストを初期化する
		/// </summary>
		public void InitPlayersHistory()
		{
#if DEBUG
			// TODO:ファイルから読み込む
			PlayersHistory.Add(new PlayerModel("ガンズ", 664, 425));
			PlayersHistory.Add(new PlayerModel("GAF", 942, 656));
			PlayersHistory.Add(new PlayerModel("いざよい", 999, 702));
			PlayersHistory.Add(new PlayerModel("ピエロ", 720, 512));
#endif
		}
	}
}
