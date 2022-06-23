using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Converters
{
	public static class ConverterFunction
	{
		public static void DoBind(DependencyObject depend, DependencyProperty property, Object source)
		{
			if (property == null) throw new ArgumentNullException("property");
			Binding binder = new Binding(property.Name);
			binder.Source = source;
			binder.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(depend, property, binder);
		}
	}

	#region ### 数値 ###
	#region Doubleの正値・負値を反転するコンバーター
	public class InvertDoubleSignConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (double)value * -1.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (double)value * -1.0;
		}
	}
	#endregion

	#region 逆数に変換するコンバーター
	public class ReciprocalConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double && (double)value != 0.0)
			{
				return 1.0 / (double)value;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double && (double)value != 0.0)
			{
				return 1.0 / (double)value;
			}
			return value;
		}
	}
	#endregion

	#region Double値を2倍にするコンバーター
	public class DoubleTwiceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
			{
				return (double)value * 2.0;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
			{
				return (double)value / 2.0;
			}
			return value;
		}
	}
	#endregion

	#region 比率をdoubleからパーセントに変換するコンバーター
	public class DoubleToPercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && value is double dbl)
			{
				return (int)Math.Round(dbl * 100.0);
			}
			return Binding.DoNothing;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null)
			{
				return (double)value / 100.0;
			}
			return Binding.DoNothing;
		}
	}
	#endregion

	#endregion

	#region ### boolian ### 
	#region bool 値を反転するコンバーター
	public class InvertBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool bValue)
			{
				return !bValue;
			}
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool bValue)
			{
				return !bValue;
			}
			return value;
		}
	}
	#endregion

	#region null のときにfalseを返すコンバーター
	public class BoolNullConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return false;
			}
			return true;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region パラメーター文字列とValueのクラスが一致したらTrue（IsInvert=trueのときはFalse)を返すコンバーター
	public class ClassNameBoolConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty IsInvertProperty =
			DependencyProperty.Register("IsInvert", typeof(bool),
				typeof(ClassNameBoolConverter), new PropertyMetadata(false));

		public bool IsInvert
		{
			get { return (bool)GetValue(IsInvertProperty); }
			set { SetValue(IsInvertProperty, value); }
		}

		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			// チェックするコンテンツタイプ文字列を取得
			String className = String.Empty;
			if( parameter is string paramstr)
			{
				className = paramstr;
			}

			//Valueがタイプと一致したら、Visibility.Visible を返す
			if (value != null && value.GetType().Name == className)
			{
				return IsInvert ? false : true;
			}
			return IsInvert ? true : false;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ClassNameBoolConverter : ConvertBack Not Supported");
		}
	}
	#endregion

	#region パラメーター文字列とValueのクラスが一致したらTrue（IsInvert=trueのときはFalse)を返すコンバーター
	//	パラメータに複数クラス名を指定できるようにした版
	public class AnyClassNameBoolConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty IsInvertProperty =
			DependencyProperty.Register("IsInvert", typeof(bool),
				typeof(AnyClassNameBoolConverter), new PropertyMetadata(false));

		public bool IsInvert
		{
			get { return (bool)GetValue(IsInvertProperty); }
			set { SetValue(IsInvertProperty, value); }
		}

		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			// チェックするコンテンツタイプ文字列を取得
			String className = String.Empty;
			if (parameter is string paramstr)
			{
				className = paramstr;
			}
			String[] split = className.Split(';');
			foreach (var p in split)
			{
				//Valueがタイプと一致したら、Visibility.Visible を返す
				if (value != null && value.GetType().Name == p)
				{
					return IsInvert ? false : true;
				}
			}

			return IsInvert ? true : false;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ClassNameBoolConverter : ConvertBack Not Supported");
		}
	}
	#endregion

	#region パラメータで指定した文字列とEnum名が一致したらTrueを返すコンバーター
	public class EnumMatchToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) throw new ArgumentNullException("value");
			if (parameter is string ParameterString)
			{
				if (Enum.IsDefined(value.GetType(), value) == false)
				{
					System.Diagnostics.Debug.Assert(false);
					return DependencyProperty.UnsetValue;
				}
				object paramvalue = Enum.Parse(value.GetType(), ParameterString);

				if (paramvalue.ToString() == value.ToString())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter is string ParameterString)
			{
				if (value is bool b && b)
				{
					return Enum.Parse(targetType, ParameterString);
				}
			}
			return Binding.DoNothing;
		}
	}
	#endregion

	#region 複数のboolがすべてtrueなら true を返す
	public class AllBoolTrueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values == null) throw new ArgumentNullException("values");

			foreach (var val in values)
			{
				if (val is bool b)
				{
					// false が混ざってたらfalseを返す
					if (b == false)
					{
						return false;
					}
				}
				else
				{
					// bool型以外が指定されたらfalseを返す
					return false;
				}
			}
			return true;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion


	#endregion

	#region ### 色 ###

	#region  ColorをBrushに変換するコンバーター（Color → Brush）
	public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Color clr)
			{
				return new SolidColorBrush(clr);
			}
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if( value is SolidColorBrush solidBrs )
			{
				return solidBrs.Color;
			}

			// solidColorBrushじゃないときは白を返す（要仕様検討）
			return Colors.White;
		}
	}
	#endregion

	#region BrushをColorに変換するコンバーター（Brush → Color ）
	public class BrushToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if( value is SolidColorBrush solidBrs )
			{
				return solidBrs.Color;
			}

			// solidColorBrushじゃないとき
			return Colors.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is Color)
			{
				return new SolidColorBrush((Color)value);
			}
			return new SolidColorBrush(Colors.White);
		}
	}
	#endregion

	#region  指定したα値で色を設定するコンバーター
	public class FixedAlphaColorConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty AlphaValueProperty =
		DependencyProperty.Register("AlphaValue", typeof(byte),
			typeof(FixedAlphaColorConverter), new PropertyMetadata((byte)0xff));

		public byte AlphaValue
		{
			get { return (byte)GetValue(AlphaValueProperty); }
			set { SetValue(AlphaValueProperty, value); }
		}


		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Color retval = new Color();
			if (value is Color)
			{
				Color val = (Color)value;
				retval.R = val.R;
				retval.G = val.G;
				retval.B = val.B;
				retval.A = AlphaValue;
			}
			return retval;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Color retval = new Color();
			if (value is Color)
			{
				Color val = (Color)value;
				retval.R = val.R;
				retval.G = val.G;
				retval.B = val.B;
				retval.A = AlphaValue;
			}
			return retval;
		}
	}
	#endregion

	#region  ImageSourceをImageBrushに変換するコンバーター
	public class ImageToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if( value is ImageSource imgSource )
			{
				return new ImageBrush(imgSource);
			}
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region  Color+α値をBrushに変換するマルチコンバーター（Color → Brush）
	public class AlphaColorToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values == null) throw new ArgumentNullException("values");
			if (values[0] is SolidColorBrush && values[1] is double)
			{
				SolidColorBrush brs = (SolidColorBrush)values[0];
				brs.Opacity = (double)values[1];
				return brs;
			}
			return new SolidColorBrush(Color.FromRgb(255, 255, 255));
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region byte(0~255)を%に変換するコンバーター（255→0%/0→100%）
	public class AlphaValue_ByteToPercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is byte)
			{
				byte byteValue = (byte)(value);
				return (double)((byteValue * 100.0) / 255.0);
			}
			return 100.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
			{
				double percentValue = (double)value;
				return (byte)(percentValue * 2.550);
			}
			return 255;
		}
	}
	#endregion

	#endregion

	#region ### 文字列 ###
	#region 文字列がnull or 空ならVisibility.Collapsedに変換するコンバーター
	public class StringVisibilityConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (value is String sourceName)
			{
				if (String.IsNullOrEmpty(sourceName))
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("StringVisibilityConverter：ConvertBack Not Supported");
		}
	}
	#endregion

	#region 文字列がnull or 空なら false を返すコンバーター
	public class StringBoolConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (value is String sourceName)
			{
				if (String.IsNullOrEmpty(sourceName))
				{
					return (IsInvert) ? true : false;
				}
			}
			return (IsInvert) ? false : true;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region フルパス文字列からファイル名だけを返すコンバーター
	public class PathToFilenameConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			if (value is String path)
			{
				if (String.IsNullOrEmpty(path) == false)
				{
					return Path.GetFileName(path);
				}
			}
			return String.Empty;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#endregion

	#region ### コントロールのVisibility ###
	#region  bool値とVisibilityを変換するコンバーター
	public class BoolVisibilityConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			// 変換値が bool 型であるか確認をします。   
			if (value is bool)
			{
				if ((bool)value == false)
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			// 変換前の値が Visibility であるか確認をします。   
			if (value is Visibility)
			{
				if ((Visibility)value == Visibility.Visible)
				{
					return (IsInvert) ? false : true;
				}
			}
			return (IsInvert) ? true : false;
		}
	}
	#endregion

	#region Objectがnullなら、Visibility.Collapsedに変換するコンバーター
	public class NullVisibilityConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (value == null)
			{
				return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region 2つのboolどちらかがFlalseなら、Visibility.Collapsedに変換するコンバーター
	public class TwoBoolVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values == null) throw new ArgumentNullException("values");

			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (values[0] is bool && values[1] is bool)
			{
				if ((bool)values[0] == false || (bool)values[1] == false)
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region intが0なら、Visibility.Collapsedに変換するコンバーター
	public class Int0VisibilityConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (value is int)
			{
				int count = (int)value;
				if (count == 0)
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	public class Int3VisibilityConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (value is int)
			{
				int count = (int)value;
				if (count == 3)
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region GridLengthが0なら、Visibility.Collapsedに変換するコンバーター
	public class GridLength0VisibilityConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (value is GridLength)
			{
				var dval = (GridLength)value;
				if (dval.Value == 0)
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion


	#region 2つのintが共に０なら、Visibility.Collapsedに変換するコンバーター
	public class Int2VisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values == null) throw new ArgumentNullException("values");

			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			if (values[0] is int && values[1] is int)
			{
				if ((int)values[0] == 0 && (int)values[1] == 0)
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region  文字スタイルとVisibilityを変換するコンバーター（文字列が空 or Normal なら Collapsed）
	public class TextStyleVisibilityConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}
			if (value is string)
			{
				string style = (string)value;
				style = style.ToLower();
				if (string.IsNullOrEmpty(style) || style == "normal")
				{
					return (IsInvert) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return (IsInvert) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool IsInvert = false;
			if (parameter != null && parameter.ToString() == "Invert")
			{
				IsInvert = true;
			}

			// 変換前の値が Visibility であるか確認をします。   
			if (value is Visibility)
			{
				if ((Visibility)value == Visibility.Visible)
				{
					return (IsInvert) ? false : true;
				}
			}
			return (IsInvert) ? true : false;
		}
	}
	#endregion

	#region チェックボックスとVisibilityを変換するコンバーター
	public class VisibleCheckedConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// 返還前の値が bool 型であるか確認をします。   
			if (value != null && value is bool)
			{
				if ((bool)value == false)
					return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// 変換前の値が Visibility であるか確認をします。   
			if (value is Visibility)
			{
				if ((Visibility)value == Visibility.Visible)
					return true;
			}
			return false;
		}
	}
	#endregion

	#region value（Enum）名称と、parameterで指定した文字列が一致したらVisiliblityを返すコンバーター
	public class EnumMatchToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if( parameter is string ParameterString)
			{
				object paramvalue = Enum.Parse(value.GetType(), ParameterString);
				if (paramvalue.ToString() == value.ToString())
				{
					return Visibility.Visible;
				}
			}
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#region パラメーター文字列とValueのクラスが一致したらVisibleを返すコンバーター
	public class ClassNameVisibilityConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty IsInvertProperty =
			DependencyProperty.Register("IsInvert", typeof(bool),
				typeof(ClassNameVisibilityConverter), new PropertyMetadata(false));

		public bool IsInvert
		{
			get { return (bool)GetValue(IsInvertProperty); }
			set { SetValue(IsInvertProperty, value); }
		}

		public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			// チェックするコンテンツタイプ文字列を取得
			var className = String.Empty;
			if (parameter != null)
			{
				className = parameter.ToString();
			}

			//Valueがタイプと一致したら、Visibility.Visible を返す
			if (value != null && value.GetType().Name == className)
			{
				return IsInvert ? Visibility.Collapsed : Visibility.Visible;
			}
			return IsInvert ? Visibility.Visible : Visibility.Collapsed;
		}
		public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}
	#endregion

	#endregion

	public class ObjectMatchVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values[0] == values[1])
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	public class StringEmptyToZeroConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is String inputStr)
			{
				if (String.IsNullOrEmpty(inputStr))
				{
					return 0;
				}
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is String inputStr)
			{
				if (String.IsNullOrEmpty(inputStr))
				{
					return 0;
				}
			}
			return value;
		}
	}
}
