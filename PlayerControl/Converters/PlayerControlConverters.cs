using System;
using System.Windows.Data;

namespace Converters
{
	public class PlayerControlConverters
	{
		public class IsBestScoreUpdateConverter : IMultiValueConverter
		{
			public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (values[0] is int selfBest)
				{
					if (values[0] is int score)
					{
						if (selfBest < score)
						{
							return true;
						}
					}
				}
				return false;
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException("ConvertBack Not Supported");
			}
		}
	}
}
