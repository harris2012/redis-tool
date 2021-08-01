using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RedisTool.Controls
{
    /// <summary>
    /// ProcessingBar.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty StatusItemProperty = DependencyProperty.Register("StatusItem",
            typeof(StatusItem),
            typeof(StatusBar),
            new PropertyMetadata(new StatusItem { Status = Status.Ready, Message = string.Empty }, new PropertyChangedCallback(StatusItemChangedCallback)));

        public static void StatusItemChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var statusBar = d as StatusBar;
            var statusItem = (StatusItem)e.NewValue;

            if (statusItem != null)
            {
                switch (statusItem.Status)
                {
                    case Status.Ready:
                        {
                            statusBar.ProcessingRing.Visibility = Visibility.Collapsed;
                            statusBar.ProcessingRing.IsActive = false;
                            statusBar.MessageTextBlock.Text = "Ready.";
                        }
                        break;
                    case Status.Processing:
                        {
                            statusBar.ProcessingRing.Visibility = Visibility.Visible;
                            statusBar.ProcessingRing.IsActive = true;
                            statusBar.MessageTextBlock.Text = "Processing.";
                        }
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(statusItem.Message))
                {
                    statusBar.MessageTextBlock.Text = statusItem.Message;
                }
            }
        }

        public StatusItem StatusItem
        {
            get
            {
                return (StatusItem)GetValue(StatusItemProperty);
            }
            set
            {
                SetValue(StatusItemProperty, value);
            }
        }
    }
}
