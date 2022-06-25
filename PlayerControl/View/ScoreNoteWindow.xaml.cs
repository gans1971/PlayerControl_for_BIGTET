using MahApps.Metro.Controls;
using PlayerControl.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlayerControl.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class ScoreNoteWindow : MetroWindow
	{
		public ScoreNoteWindow()
		{
			InitializeComponent();
		}

		private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
		{
			if (DataContext is MainWindowViewModel vm && sender is ListBoxItem item)
			{
				switch (e.Key)
				{
					case Key.Enter:
						vm.InputScore(item.DataContext);
						e.Handled = true;
						break;
					case Key.Left:
					case Key.NumPad1:
					case Key.D1:
						vm.ChangePlayer(item.DataContext, true);
						e.Handled = true;
						break;
					case Key.Right:
					case Key.NumPad2:
					case Key.D2:
						vm.ChangePlayer(item.DataContext, false);
						e.Handled = true;
						break;
				}
			}
		}
	}
}
