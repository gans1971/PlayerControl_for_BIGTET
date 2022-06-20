using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using PlayerControl.Model;
using PlayerControl.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace PlayerControl.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
		{
			if (DataContext is MainWindowViewModel vm && sender is ListBoxItem item)
			{
				switch (e.Key)
				{
					case Key.Enter:
						vm.SaveStreamControlJson();
						item.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate ()
						{
							item.Focus();
						}));

						e.Handled = true;
						break;
					case Key.Space:
						vm.InputScore(item.DataContext);
						e.Handled = true;
						break;
					case Key.Left:
						vm.ChangePlayer(item.DataContext, true);
						e.Handled = true;
						break;
					case Key.Right:
						vm.ChangePlayer(item.DataContext, false);
						e.Handled = true;
						break;
					case Key.C:
						if (Keyboard.Modifiers == ModifierKeys.Control)
						{
							vm.SetPlayerInfoToClipboard(item.DataContext);
							e.Handled = true;
						}
						break;
				}
			}
		}

		private void ScoreTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			InputMethod.Current.ImeState = InputMethodState.Off;
		}

		private void ScoreTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// 0-9のみ
			e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
		}

		private void ScoreTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// 貼り付けを許可しない
			if (e.Command == ApplicationCommands.Paste)
			{
				e.Handled = true;
			}
		}
	}
}
