using MahApps.Metro.Controls;
using PlayerControl.ViewModels;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PlayerControl.View
{
	/// <summary>
	/// Interaction logic for PlayerControlWindow.xaml
	/// </summary>
	public partial class PlayerControlWindow : MetroWindow
	{
		public PlayerControlWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// アイテム選択中のキーボード操作
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
		{
			if (DataContext is MainViewModel vm && sender is ListBoxItem item)
			{
				switch (e.Key)
				{
					// 
					case Key.Enter:
						vm.SaveStreamControlJson();
						item.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate ()
						{
							item.Focus();
						}));

						e.Handled = true;
						break;

					// スコア入力ダイアログを表示
					case Key.Space:
						vm.InputScore(item.DataContext);
						e.Handled = true;
						break;

					// 1Pに設定
					case Key.Left:
						vm.ChangePlayer(item.DataContext, true);
						e.Handled = true;
						break;

					// 2Pに設定
					case Key.Right:
						vm.ChangePlayer(item.DataContext, false);
						e.Handled = true;
						break;

					// クリップボードにユーザー名とスコアをコピー
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

		/// <summary>
		/// スコアテキストボックスにキーボードフォーカスが当たったときの処理
		/// IMEをOFFにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScoreTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			InputMethod.Current.ImeState = InputMethodState.Off;
		}

		/// <summary>
		/// スコアテキストボックスの文字入力処理（Preview）
		/// 数字のみ入力できるようにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScoreTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// 0-9のみ
			e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
		}

		/// <summary>
		/// スコアテキストボックスへのコマンド処理(Preview)
		/// 外部から文字をペーストできないようにする（数字のみを許容するため）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
