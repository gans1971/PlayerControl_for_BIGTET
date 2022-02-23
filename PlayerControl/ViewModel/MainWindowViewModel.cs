using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PlayerControl.Model;
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
		private static String _htmlSourceFile = "scoreboard.html";

		public IDialogCoordinator MahAppsDialogCoordinator { get; set; } = new DialogCoordinator();

		#region ReactiveProperty
		public ReactivePropertySlim<string> targetHandle { get; } = new ReactivePropertySlim<string>(String.Empty);
		public ReactivePropertySlim<PlayerModel> SelectedPlayer { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrnetPlayer1 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactivePropertySlim<PlayerModel> CurrnetPlayer2 { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactiveCollection<PlayerModel> Players { get; } = new ReactiveCollection<PlayerModel>();

		#endregion

		#region ReactiveCommand
		public ReactiveCommand Player1ChangeCommand { get; }
		public ReactiveCommand Player2ChangeCommand { get; }
		public ReactiveCommand AboutBoxCommand { get; }
		public ReactiveCommand LoadedCommand { get; }
		#endregion


		private bool SaveStremControlJson(RoutedEventArgs args, String jsonFolder, bool isPlayer1)
		{
			if (args.Source is FrameworkElement fe)
			{
				if (fe.DataContext is PlayerModel player)
				{
					if(isPlayer1)
					{
						CurrnetPlayer1.Value = player;
					}
					else
					{
						CurrnetPlayer2.Value = player;
					}
					return SaveStreamControlJson(jsonFolder);
				}
			}
			return false;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel()
		{
			var jsonPath = GetStreamControlPath();

			// プレイヤー1切り替えコマンド
			Player1ChangeCommand = new ReactiveCommand();
			Player1ChangeCommand.Subscribe(x =>
			{
			   if (x is RoutedEventArgs args)
			   {
					SaveStremControlJson(args, jsonPath, true);
			   }
			}).AddTo(Disposable);

			// プレイヤー2切り替えコマンド
			Player2ChangeCommand = new ReactiveCommand();
			Player2ChangeCommand.Subscribe(x =>
			{
				if (x is RoutedEventArgs args)
				{
					SaveStremControlJson(args, jsonPath, false);
				}
			}).AddTo(Disposable);

			LoadedCommand = new ReactiveCommand();
			LoadedCommand.Subscribe(_ =>
			{
				InitPlayers();
			}).AddTo(Disposable);

			// ReactiveCommand Subscribe
			AboutBoxCommand = new ReactiveCommand();
			AboutBoxCommand.Subscribe(async _ =>
			{
				var _asmAttr = new Servicies.AssemblyAttribute();
				var _envInfo = new Servicies.EnvironmentInfo();
				await this.MahAppsDialogCoordinator.ShowMessageAsync(this, $"{_envInfo.WindowsCaption}", $"Ver.{_envInfo.WindowsVersion} Now = {DateTime.Now}");
			});
		}


		/// <summary>
		/// MainWindow初期化時にコールされる初期化処理
		/// </summary>
		public void Initialize()
		{
			InitPlayers();
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
		/// <param name="jsonpath"></param>
		/// <returns></returns>
		public bool SaveStreamControlJson(String jsonpath)
		{
			try
			{
				// ファイルパスチェック
				if (!Directory.Exists(jsonpath))
				{
					return false;
				}
				var savepath = System.IO.Path.Combine(jsonpath, "streamcontrol.json");

				// Jsonクラスに値をセット

				var StreamControlData = new StreamControlParam();
				if (CurrnetPlayer1.Value != null)
				{
					StreamControlData.pName1 = CurrnetPlayer1.Value.Name.Value;
					StreamControlData.pScore1 = CurrnetPlayer1.Value.TodayBest.Value.ToString();
				}
				if (CurrnetPlayer2.Value != null)
				{
					StreamControlData.pName2 = CurrnetPlayer2.Value.Name.Value;
					StreamControlData.pScore2 = CurrnetPlayer2.Value.TodayBest.Value.ToString();
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
		/// プレイヤーリストを初期化する
		/// </summary>
		public void InitPlayers()
		{
			Players.Add(new PlayerModel("ガンズ", 664, 425));
			Players.Add(new PlayerModel("GAF", 942, 656));
			Players.Add(new PlayerModel("まつのゆ", 580, 530));
			Players.Add(new PlayerModel("いにゅうえんどう", 999, 702));
			Players.Add(new PlayerModel("チャーハンライス", 999, 702));
		}
	}
}
