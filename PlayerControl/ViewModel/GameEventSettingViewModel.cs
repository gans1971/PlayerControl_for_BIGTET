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

namespace PlayerControl.ViewModels
{
	public class GameEventSettingViewModel : BindableViewModel
	{
		ReactivePropertySlim<String> EventTitle { get; }
		public GameEventSettingViewModel()
		{
			EventTitle = new ReactivePropertySlim<string>(String.Empty);
		}
	}
}
