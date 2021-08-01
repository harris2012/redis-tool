using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace RedisTool
{
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class VisibilityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;

            bool? source = (bool?)value;

            if (value != null)
            {
                if (source.HasValue && source.Value)
                {
                    visibility = Visibility.Visible;
                }
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
