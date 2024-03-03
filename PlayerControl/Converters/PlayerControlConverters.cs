using PlayerControl.Model;
using System;
using System.Windows;
using System.Windows.Data;

namespace Converters
{

	/// <summary>
	/// スコアのラベル文字列をモード別に返すコンバーター
	/// </summary>
	public class Score_Second_LavelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ScoreMode mode)
			{
				switch (mode)
				{
					case ScoreMode.Mixture:
						return "20G";
					default:
						return "LV";
				}
			}
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	/// <summary>
	/// スコアのラベル文字列のデフォルト値を返すコンバーター
	/// プロパティが空の場合は "Score" を返す
	/// </summary>
	public class Score_LavelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is String label)
			{
				if( String.IsNullOrEmpty(label) )
				{
					return "Score";
				}
				else
				{
					return label;
				}
			}
			return String.Empty;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	/// <summary>
	/// スコアモードがシングルの場合にVisibility.Collapsedを返すコンバーター
	/// </summary>
	public class AddLabelConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var retStr = String.Empty;
			if (value is String body)
			{
				if (parameter is String label)
				{
					retStr = label;
				}
				if (!String.IsNullOrEmpty(body))
				{
					retStr += body;
				}
			}
			return retStr;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	public class ScoreModeToVisinilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ScoreMode mode)
			{
				if (mode != ScoreMode.Single)
				{
					return Visibility.Visible;
				}
			}
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	/// <summary>
	/// 列挙されたスコアの合計を計算するコンバーター
	/// </summary>
	public class ScoreAmountConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var amount = 0;
			foreach (var value in values)
			{
				if(value is int score)
				{
					amount += score;
				}
			}
			return amount.ToString();
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	/// <summary>
	/// 配列中に有効なプレイヤー名があればVisibilityを返すコンバーター
	/// </summary>
	public class EnablePlayerNamesVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			foreach (var value in values)
			{
				if (value is String name)
				{
					// 一人でも有効な名前があれば表示する
					if (!String.IsNullOrEmpty(name))
					{
						return Visibility.Visible;
					}
				}
			}
			return Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

	/// <summary>
	/// Valueが1以上のときにTrueを返すコンバーター
	/// </summary>
	public class CountBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int count)
			{
				if (count > 0)
				{
					return true;
				}
			}
			return false;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException("ConvertBack Not Supported");
		}
	}

}
