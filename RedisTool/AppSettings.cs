using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;

namespace RedisTool
{
    public class AppSettings : DependencyObject
    {
        public static readonly DependencyProperty RedisProviderProperty = DependencyProperty.Register(nameof(RedisProvider), typeof(string), typeof(AppSettings), new PropertyMetadata(ConfigurationManager.AppSettings["RedisProvider"]));

        public static readonly DependencyProperty ComfirmRequiredProperty = DependencyProperty.Register(nameof(ComfirmRequired), typeof(bool), typeof(AppSettings), new PropertyMetadata(bool.Parse(ConfigurationManager.AppSettings["ComfirmRequired"])));

        public string RedisProvider
        {
            get
            {
                return (string)GetValue(RedisProviderProperty);
            }
            set
            {
                SetValue(RedisProviderProperty, value);
            }
        }

        public bool ComfirmRequired
        {
            get
            {
                return (bool)GetValue(ComfirmRequiredProperty);
            }
            set
            {
                SetValue(ComfirmRequiredProperty, value);
            }
        }
    }
}
