using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlayerControl.View
{
	/// <summary>
	/// ScoreSettingView.xaml の相互作用ロジック
	/// </summary>
	public partial class ScoreSettingView : UserControl
	{
		public ScoreSettingView()
		{
			InitializeComponent();
		}

		private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			InputMethod.Current.ImeState = InputMethodState.Off;
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// 0-9のみ
			e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
		}

		private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// 貼り付けを許可しない
			if (e.Command == ApplicationCommands.Paste)
			{
				e.Handled = true;
			}
		}
	}

	public class NotEmptyValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			return string.IsNullOrWhiteSpace((value ?? "").ToString())
				? new ValidationResult(false, "スコアを入力してください")
				: ValidationResult.ValidResult;
		}
	}
}
