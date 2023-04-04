using System;
using System.Windows;

namespace PlayerControl
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var senderStr = "[sender null]";
			if( sender != null)
			{
				senderStr = sender.ToString();
			}

			MessageBox.Show(
				"CurrentDomain_UnhandledException() error\n" +
				 senderStr + "→" + e.ExceptionObject.ToString(),
				"アプリケーション例外が発生",
				  MessageBoxButton.OK,
				MessageBoxImage.Error);
		}

	}
}
