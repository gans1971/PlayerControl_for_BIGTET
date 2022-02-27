// TextBoxGotFocusBehaviors.cs
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MahApps;
using MahApps.Metro.Controls;

namespace Behavior
{
	public class TextBoxGotFocusBehaviors
	{
		public static readonly DependencyProperty SelectAllOnGotFocusProperty =
				DependencyProperty.RegisterAttached(
						"SelectAllOnGotFocus",
						typeof(bool),
						typeof(TextBoxGotFocusBehaviors),
						new UIPropertyMetadata(false, SelectAllOnGotFocusChanged)
				);

		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static bool GetSelectAllOnGotFocus(DependencyObject obj)
		{
			return (bool)obj.GetValue(SelectAllOnGotFocusProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static void SetSelectAllOnGotFocus(DependencyObject obj, bool value)
		{
			obj.SetValue(SelectAllOnGotFocusProperty, value);
		}

		private static void SelectAllOnGotFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs evt)
		{
			if (sender is TextBox textBox)
			{
				textBox.GotFocus -= OnTextBoxGotFocus;
				if ((bool)evt.NewValue)
				{
					textBox.GotFocus += OnTextBoxGotFocus;
				}
			}
		}

		private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				textBox.Dispatcher.BeginInvoke((Action)(() => textBox.SelectAll()));
			}
		}
	}
	public class NumericUpDownGotFocusBehaviors
	{
		public static readonly DependencyProperty SelectAllOnGotFocusProperty =
				DependencyProperty.RegisterAttached(
						"SelectAllOnGotFocus",
						typeof(bool),
						typeof(NumericUpDownGotFocusBehaviors),
						new UIPropertyMetadata(false, SelectAllOnGotFocusChanged)
				);

		[AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
		public static bool GetSelectAllOnGotFocus(DependencyObject obj)
		{
			return (bool)obj.GetValue(SelectAllOnGotFocusProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
		public static void SetSelectAllOnGotFocus(DependencyObject obj, bool value)
		{
			obj.SetValue(SelectAllOnGotFocusProperty, value);
		}

		private static void SelectAllOnGotFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs evt)
		{
			if (sender is NumericUpDown nud)
			{
				nud.GotFocus -= OnNumericUpDownGotFocus;
				if ((bool)evt.NewValue)
				{
					nud.GotFocus += OnNumericUpDownGotFocus;
				}
			}
		}

		private static void OnNumericUpDownGotFocus(object sender, RoutedEventArgs e)
		{
			if (sender is NumericUpDown nud)
			{
				nud.Dispatcher.BeginInvoke((Action)(() => nud.SelectAll()));
			}
		}
	}
}