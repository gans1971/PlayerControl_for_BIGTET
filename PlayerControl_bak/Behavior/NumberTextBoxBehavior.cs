using Microsoft.Xaml.Behaviors;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Behavior
{
	class NumberTextBoxBehavior : Behavior<TextBox>
	{
		private Regex reg = new Regex("^[0-9]*$");

		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
			AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
			DataObject.AddPastingHandler(this.AssociatedObject, PastingHandler);

			// IMEを無効化
			InputMethod.SetIsInputMethodSuspended(AssociatedObject, true);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
			AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
			DataObject.RemovePastingHandler(this, PastingHandler);
			InputMethod.SetIsInputMethodSuspended(AssociatedObject, false);
		}

		private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// 数字以外の入力を拒否
			if (!reg.IsMatch(e.Text))
			{
				e.Handled = true;
			}
		}

		private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// PreviewTextInputでは半角スペースを検知できないので、PreviewKeyDownで検知
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private void PastingHandler(object sender, DataObjectPastingEventArgs e)
		{
			// 貼り付け時に数字以外の入力を拒否
			var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
			if (!isText) return;

			if (e.SourceDataObject.GetData(DataFormats.UnicodeText) is string text)
			{
				if (!reg.IsMatch(text))
				{
					e.CancelCommand();
					e.Handled = true;
				}
			}
		}
	}
	class HexTextBoxBehavior : Behavior<TextBox>
	{
		private Regex reg = new Regex("^[a-fA-F0-90-9]*$");

		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
			AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
			DataObject.AddPastingHandler(this.AssociatedObject, PastingHandler);

			// IMEを無効化
			InputMethod.SetIsInputMethodSuspended(AssociatedObject, true);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
			AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
			DataObject.RemovePastingHandler(this, PastingHandler);
			InputMethod.SetIsInputMethodSuspended(AssociatedObject, false);
		}

		private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// 数字以外の入力を拒否
			if (!reg.IsMatch(e.Text))
			{
				e.Handled = true;
			}
		}

		private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// PreviewTextInputでは半角スペースを検知できないので、PreviewKeyDownで検知
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private void PastingHandler(object sender, DataObjectPastingEventArgs e)
		{
			// 貼り付け時に数字以外の入力を拒否
			var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
			if (!isText) return;

			if (e.SourceDataObject.GetData(DataFormats.UnicodeText) is string text)
			{
				if (!reg.IsMatch(text))
				{
					e.CancelCommand();
					e.Handled = true;
				}
			}
		}
	}
}