using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RedisTool.Pages
{
    /// <summary>
    /// Interaction logic for RemovePage.xaml
    /// </summary>
    public partial class RemovePage : UserControl
    {
        public RemovePage()
        {
            InitializeComponent();
        }

        private void RemoveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(ProviderTextBox.Text)
                && !string.IsNullOrEmpty(KeyTextBox.Text);
        }

        private void RemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ComfirmRequiredCheckBox.IsChecked != null
                && ComfirmRequiredCheckBox.IsChecked.Value
                && MessageBox.Show("Are you sure to remove?", "Comfirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var providerName = ProviderTextBox.Text;

            string key = KeyTextBox.Text;

            SetStatus(Status.Processing, string.Empty);

            ThreadPool.QueueUserWorkItem(new WaitCallback(item =>
            {
                var returnValue = Process(providerName, key);

                //var output = string.Format(AppConst.OutputTemplate, value, DateTime.Now);

                this.Dispatcher.Invoke(new Action(() =>
                {
                    OutputTextBox.Text = returnValue;

                    SetStatus(Status.Ready, string.Empty);
                }));
            }), 21);
        }

        private string Process(string providerName, string key)
        {
            string returnValue = string.Empty;

            try
            {
                var provider = RedisProvider.GetConnectionMultiplexer(providerName).GetDatabase();

                var result = provider.KeyDelete(key);

                if (result)
                {
                    returnValue = string.Format("Redis remove successfully.");
                }
                else
                {
                    returnValue = string.Format("Redis remove failed.");
                }

            }
            catch (Exception ex)
            {
                returnValue = ex.ToString();
            }

            return returnValue;
        }

        private void SetStatus(Status status, string message)
        {
            StatusBar.StatusItem = new StatusItem { Status = status, Message = message };

            switch (status)
            {
                case Status.Ready:
                    this.IsEnabled = true;
                    break;
                case Status.Processing:
                    this.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}
