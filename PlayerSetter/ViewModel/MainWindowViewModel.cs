using MahApps.Metro.Controls.Dialogs;
using PlayerSetter.Model;
using PlayerSetter.ViewModels;
using Reactive.Bindings;
using System;

namespace PlayerSetter.ViewModels
{
    public class MainWindowViewModel : BindableViewModel
    {
        public IDialogCoordinator MahAppsDialogCoordinator { get; set; } = new DialogCoordinator();

        public ReactivePropertySlim<string> targetHandle { get; } = new ReactivePropertySlim<string>(String.Empty);
		public ReactivePropertySlim<PlayerModel> SelectedPlayer { get; } = new ReactivePropertySlim<PlayerModel>();
		public ReactiveCollection<PlayerModel> Players { get; } = new ReactiveCollection<PlayerModel>();


		public ReactiveCommand AboutBoxCommand { get; }

		public MainWindowViewModel()
		{
			AboutBoxCommand = new ReactiveCommand();
			AboutBoxCommand.Subscribe(async _=>
			{
				var _asmAttr = new Servicies.AssemblyAttribute();
				var _envInfo = new Servicies.EnvironmentInfo();
				await this.MahAppsDialogCoordinator.ShowMessageAsync(this, $"{_envInfo.WindowsCaption}", $"Ver.{_envInfo.WindowsVersion} Now = {DateTime.Now}" );
			});

		}
	}
}
