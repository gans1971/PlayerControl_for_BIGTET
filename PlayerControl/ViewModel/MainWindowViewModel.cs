using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using PlayerControl.Model;
using PlayerControl.View;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PlayerControl.ViewModels
{
	public class MainWindowViewModel : BindableViewModel
	{
		private const String _htmlSourceFile = "scoreboard.html";
		private const String _jsonFileName = "streamcontrol.json";

		// CurrentPlayerをクリアするためのインスタンス
		private PlayerModel _emptyPlayer { get; } = new PlayerModel();

		public IDialogCoordinator MahAppsDialogCoordinator { get; set; } = new DialogCoordinator();

		#region ReactiveProperty
		public ReactivePropertySlim<string> targetHandle { get; } = new ReactivePropertySlim<string>(String.Empty);
		public ReactivePropertySlim<PlayerModel> SelectedPlayer { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrentPlayer1 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrentPlayer2 { get; } = new ReactivePropertySlim<PlayerModel>();

		public ReactivePropertySlim<GameEventSettingViewModel> EventSetting { get; } = new ReactivePropertySlim<GameEventSettingViewModel>();
		public ReactiveCollection<PlayerModel> Players { get; } = new ReactiveCollection<PlayerModel>();
		public ReactiveCollection<PlayerModel> PlayersHistory { get; } = new ReactiveCollection<PlayerModel>();
		#endregion

		#region ReactiveCommand
		public ReactiveCommand Player1ChangeCommand { get; }
		public ReactiveCommand Player2ChangeCommand { get; }
		public ReactiveCommand AboutBoxCommand { get; }
		public ReactiveCommand LoadedCommand { get; }
		public ReactiveCommand ClosingCommand { get; }
		public ReactiveCommand OpenGameEventSettingCommand { get; }
		public ReactiveCommand EditPlayersListCommand { get; }
		public ReactiveCommand RemovePlayerCommand { get; }
		public ReactiveCommand SetTodayBestCommand { get; }
		public ReactiveCommand InputScoreCommand { get; }
		public ReactiveCommand<Object> AddPlayerCommand { get; }
		public ReactiveCommand PlayerExchangeCommand { get; }
		public ReactiveCommand<string> PlayerClearCommand { get; }
		public ReactiveCommand SaveJsonCommand { get; }
		
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel()
		{
			// アプリ開始コマンド
			LoadedCommand = new ReactiveCommand();
			LoadedCommand.Subscribe(_ =>
			{
				// 初期状態でStreamControl Jsonファイルを保存
				SaveStreamControlJson();

				// プレイヤー履歴を初期化
				InitPlayersHistory();

			}).AddTo(Disposable);
			// アプリ終了前コマンド
			ClosingCommand = new ReactiveCommand();
			ClosingCommand.Subscribe(_ =>
			{
				// StreamControl Jsonファイルを初期化（タイムスタンプも初期化）
				CurrentPlayer1.Value = new PlayerModel();
				CurrentPlayer2.Value = new PlayerModel();
				SaveStreamControlJson(true);

			}).AddTo(Disposable);

			// イベント設定画面を表示するコマンド
			OpenGameEventSettingCommand = new ReactiveCommand();
			OpenGameEventSettingCommand.Subscribe(async _ =>
			{
				var view = new GameEventSettingView()
				{
					DataContext = EventSetting.Value
				};
				// ダイアログ外をクリックして閉じた場合はnullが返ってくる
				var result = await DialogHost.Show(view, "MainWindowDialog");
				if (result is bool settingResult)
				{
				}
			}).AddTo(Disposable);

			// プレイヤー編集コマンド
			EditPlayersListCommand = new ReactiveCommand();
			EditPlayersListCommand.Subscribe(_ =>
			{
				var playerEditWindow = new PlayersEditWindow();
				playerEditWindow.DataContext = this;
				playerEditWindow.Show();
			}).AddTo(Disposable);

			// 本日ベスト変更コマンド
			SetTodayBestCommand = new ReactiveCommand();
			SetTodayBestCommand.Subscribe(x =>
		   {
				// イベント発火元コントロールのDataContextからVMを取得して更新
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe)
			   {
				   InputScore(fe.DataContext);
			   }
		   }).AddTo(Disposable);

			// プレイヤー1切り替えコマンド
			Player1ChangeCommand = new ReactiveCommand();
			Player1ChangeCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe &&
					fe.DataContext is PlayerModel player)
				{
					CurrentPlayer1.Value = player;
					if( CurrentPlayer2.Value == player)
					{
						CurrentPlayer2.Value = _emptyPlayer;
					}
					SaveStreamControlJson();
				}
			}).AddTo(Disposable);

			// プレイヤー2切り替えコマンド
			Player2ChangeCommand = new ReactiveCommand();
			Player2ChangeCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe &&
					fe.DataContext is PlayerModel player)
				{
					CurrentPlayer2.Value = player;
					if (CurrentPlayer1.Value == player)
					{
						CurrentPlayer1.Value = _emptyPlayer;
					}
					SaveStreamControlJson();
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

			InputScoreCommand = new ReactiveCommand();
			InputScoreCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args && args.Source is FrameworkElement fe &&
					fe.DataContext is PlayerModel player)
				{
					InputScore(player);
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
				if( param is TextBox textBox )
				{
					var name = textBox.Text;
					var trimName = name.Trim();
					if(!String.IsNullOrEmpty(trimName))
					{
						Players.Add(new PlayerModel(trimName, 0, 0));
						SaveStreamControlJson();
					}
					textBox.Text = String.Empty;
				}
			}).AddTo(Disposable);

			SaveJsonCommand = new ReactiveCommand();
			SaveJsonCommand.Subscribe(_ =>
			{
				SaveStreamControlJson();
			}).AddTo(Disposable);
			

			// このアプリについて表示する
			AboutBoxCommand = new ReactiveCommand();
			AboutBoxCommand.Subscribe(async _ =>
			{
				var _asmAttr = new Servicies.AssemblyAttribute();
				var _envInfo = new Servicies.EnvironmentInfo();

				await this.MahAppsDialogCoordinator.ShowMessageAsync(this, $"{_envInfo.WindowsCaption}", $"Ver.{_envInfo.WindowsVersion} Now = {DateTime.Now}");
			});
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
					var view = new TodayBestSettingView()
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
				Debug.WriteLine($"Exception TodayBestImput:{ex.ToString()}");
			}
		}

		/// <summary>
		/// MainWindow初期化時にコールされる初期化処理
		/// </summary>
		public void Initialize()
		{

			// TODO:プレイヤーリストを読み込む
			InitPlayersHistory();
		}

		/// <summary>
		/// ソースHTMLファイルが存在するパスを取得
		/// 実行ファイルの親フォルダをたどる
		/// </summary>
		/// <returns></returns>
		private String GetStreamControlPath()
		{
			// 実行ファイルのパスを取得
			var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var di = new DirectoryInfo(appPath);
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
				var jsonPath = GetStreamControlPath();

				// ファイルパスチェック
				if (!Directory.Exists(jsonPath))
				{
					MessageBox.Show($"StreamControl の HTML保存フォルダが見つかりません");
					return false;
				}

				// Path + Jsonファイル名
				var savepath = System.IO.Path.Combine(jsonPath, _jsonFileName);

				// Jsonクラスに値をセット(CurrentPlayerがnullの場合は初期値のまま保存）
				var StreamControlData = new StreamControlParam();
				if (CurrentPlayer1.Value != null)
				{
					StreamControlData.pName1 = CurrentPlayer1.Value.Name.Value;
					StreamControlData.pScore1 = CurrentPlayer1.Value.TodayBest.Value.ToString();
				}
				if (CurrentPlayer2.Value != null)
				{
					StreamControlData.pName2 = CurrentPlayer2.Value.Name.Value;
					StreamControlData.pScore2 = CurrentPlayer2.Value.TodayBest.Value.ToString();
				}
				// タイムスタンプを初期化する場合
				if( InitTimeStump)
				{
					StreamControlData.timestamp = "0";
				}

				Console.WriteLine($"time:{StreamControlData.timestamp}");

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

			// TODO:Player追加UIができたら消す
			Players.Add(new PlayerModel("ガンズ", 664, 425));
			Players.Add(new PlayerModel("GAF", 942, 656));
			Players.Add(new PlayerModel("まつのゆ", 580, 530));
			Players.Add(new PlayerModel("いにゅうえんどう", 999, 702));
			Players.Add(new PlayerModel("いざよい", 999, 702));
			Players.Add(new PlayerModel("ピエロ", 720, 512));
#endif
		}
	}
}
