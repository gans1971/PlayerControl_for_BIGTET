using PlayerControl.Model;
using System;
using System.Windows.Data;

namespace Converters
{
	#region スコアのラベル文字列をモード別に返すコンバーター
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
	public class Score_LavelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ScoreMode mode)
			{
				switch (mode)
				{
					case ScoreMode.Mixture:
						return "Nor";
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
	#endregion
}
