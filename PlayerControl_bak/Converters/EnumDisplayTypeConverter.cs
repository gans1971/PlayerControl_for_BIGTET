using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Converters
{
    public class EnumDisplayTypeConverter : EnumConverter
    {
        public EnumDisplayTypeConverter(Type type) : base(type)
        {
        }

		public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
					var name = value.ToString();
					if (name != null)
					{
						FieldInfo? field = value.GetType().GetField(name);
						if (field != null)
						{
							var attribute = field.GetCustomAttribute<DisplayAttribute>(false);
							return attribute == null ? value.ToString() : attribute.GetName();
						}
					}
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
}
