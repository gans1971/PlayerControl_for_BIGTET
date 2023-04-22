using Converters;
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
	public partial class PlayerSettingView : UserControl
	{
		// ダイアログ内で編集するプロパティ（キャンセルできるように直接バインドしない）
		public static readonly DependencyProperty EditNameProperty =
			DependencyProperty.Register("EditName", typeof(String),
			typeof(PlayerSettingView), new PropertyMetadata(String.Empty));
		public String EditName
		{
			get { return (String)GetValue(EditNameProperty); }
			set { SetValue(EditNameProperty, value); }
		}

		public static readonly DependencyProperty EditTwitterProperty =
			DependencyProperty.Register("EditTwitter", typeof(String),
			typeof(PlayerSettingView), new PropertyMetadata(String.Empty));
		public String EditTwitter
		{
			get { return (String)GetValue(EditTwitterProperty); }
			set { SetValue(EditTwitterProperty, value); }
		}


		public PlayerSettingView()
		{
			InitializeComponent();
		}
	}

	public class NotEmptyPlayerNameValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			return string.IsNullOrWhiteSpace((value ?? "").ToString())
				? new ValidationResult(false, "プレイヤー名を入力してください")
				: ValidationResult.ValidResult;
		}
	}
}
