using PlayerControl.Model;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlayerControl.View
{
	/// <summary>
	/// ScoreSettingView.xaml の相互作用ロジック
	/// </summary>
	public partial class ScoreSettingView : UserControl
	{
		public static readonly DependencyProperty CurrentScoreModeProperty =
			DependencyProperty.Register("CurrentScoreMode", typeof(ScoreMode),	typeof(ScoreSettingView), new PropertyMetadata(ScoreMode.Single));
		public ScoreMode CurrentScoreMode
		{
			get { return (ScoreMode)GetValue(CurrentScoreModeProperty); }
			set { SetValue(CurrentScoreModeProperty, value); }
		}

		public static readonly DependencyProperty ScoreLabelText1Property =
			DependencyProperty.Register("ScoreLabelText1", typeof(String), typeof(ScoreSettingView), new PropertyMetadata(String.Empty));
		public String ScoreLabelText1
		{
			get { return (String)GetValue(ScoreLabelText1Property); }
			set { SetValue(ScoreLabelText1Property, value); }
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="_currentMode">スコアモード TODO:Ver0.3.x未実装</param>
		/// <param name="_scoreLabel1">スコアラベルに表示する文字列</param>
		public ScoreSettingView(ScoreMode _currentMode, String _scoreLabel1)
		{
			// 引数で指定されたUI設定を反映
			CurrentScoreMode = _currentMode;
			ScoreLabelText1 = _scoreLabel1;

			InitializeComponent();

			// スコアテキストボックスにフォーカスを当てる
			if (ScoreTextBox1 != null)
			{
				ScoreTextBox1.Focus();
				Dispatcher.BeginInvoke((Action)(() => ScoreTextBox1.SelectAll())); // TODO:選択状態にならない…？
			}

		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// スコアテキストのキーボードフォーカス時にIMEをOFFにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScoreTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			InputMethod.Current.ImeState = InputMethodState.Off;
		}
		/// <summary>
		/// スコアテキストボックスに数値のみ受け付けるようにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScoreTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// 0-9のみ
			e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
		}
		/// <summary>
		/// スコアテキストボックスへペーストを受け付けないようにする
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

	public class NotEmptyValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			return string.IsNullOrWhiteSpace((value ?? "").ToString())
				? new ValidationResult(false, $"スコアを入力してください")
				: ValidationResult.ValidResult;
		}
	}
}
