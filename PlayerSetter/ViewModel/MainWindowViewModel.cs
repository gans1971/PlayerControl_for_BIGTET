using MahApps.Metro.Controls.Dialogs;
using PlayerControl.Model;
using Reactive.Bindings;
using System;

namespace PlayerControl.ViewModels
{
	public class MainWindowViewModel : BindableViewModel
	{
		public IDialogCoordinator MahAppsDialogCoordinator { get; set; } = new DialogCoordinator();

		#region ReactiveProperty
		public ReactivePropertySlim<string> targetHandle { get; } = new ReactivePropertySlim<string>(String.Empty);
		public ReactivePropertySlim<PlayerModel> SelectedPlayer { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactiveCollection<PlayerModel> Players { get; } = new ReactiveCollection<PlayerModel>();
		#endregion

		#region ReactiveCommand
		public ReactiveCommand AboutBoxCommand { get; }

		public ReactiveCommand LoadedCommand { get; }
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel()
		{
			LoadedCommand = new ReactiveCommand();
			LoadedCommand.Subscribe(_ =>
			{
				InitPlayers();
			});

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


		public void InitPlayers()
		{
			Players.Add(new PlayerModel("ガンズ", 664, 425));
			Players.Add(new PlayerModel("GAF", 942, 656));
			Players.Add(new PlayerModel("まつのゆ", 580, 530));
			Players.Add(new PlayerModel("いにゅうえんどう", 999, 702));
		}
	}
}
