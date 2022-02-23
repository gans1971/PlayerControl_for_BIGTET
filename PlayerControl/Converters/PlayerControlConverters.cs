using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PlayerControl.Converters
{
	internal class PlayerControlConverters
	{


		public class IsBestScoreUpdateConverter : IMultiValueConverter
		{
			public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if( values[0] is int selfBest)
				{
					if(values[0] is int todayBest)
					{
						if(selfBest< todayBest)
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
