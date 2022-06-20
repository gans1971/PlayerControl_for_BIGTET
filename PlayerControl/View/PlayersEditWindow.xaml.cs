using MahApps.Metro.Controls;
using PlayerControl.Model;
using PlayerControl.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlayerControl.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class PlayersEditWindow : MetroWindow
	{
		public PlayersEditWindow()
		{
			InitializeComponent();
		}

		private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}

		private void PlayerNameEditButton_Click(object sender, RoutedEventArgs e)
		{
			if (DataContext is MainWindowViewModel vm && sender is FrameworkElement fe)
			{
				vm.InputPlayerName(fe.DataContext);
				e.Handled = true;
			}
			//if (sender is FrameworkElement fe && fe.DataContext is PlayerModel player)
			//{
			//	player.IsPlayerNameInEditMode.Value = true;
			//	e.Handled = true;
			//}

		}

		private void PlayerNameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is PlayerModel player)
			{
				if( e.Key == Key.Enter)
				{
					player.IsPlayerNameInEditMode.Value = false;
					e.Handled = true;
				}
			}
		}

		private void PlayerNameTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (sender is FrameworkElement fe && fe.DataContext is PlayerModel player)
			{
				player.IsPlayerNameInEditMode.Value = false;
				e.Handled = true;
			}
		}
	}
}
