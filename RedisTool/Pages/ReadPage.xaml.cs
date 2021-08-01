using Newtonsoft.Json;
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
    /// Interaction logic for ReadPage.xaml
    /// </summary>
    public partial class ReadPage : UserControl
    {
        public ReadPage()
        {
            InitializeComponent();
        }

        private void ReadCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(ProviderTextBox.Text) && !string.IsNullOrEmpty(KeyTextBox.Text);
        }

        private void ReadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var providerName = ProviderTextBox.Text;

            string key = KeyTextBox.Text;

            #region ValueType
            ValueType valueType = ValueType.String;
            if (ListRadioButton.IsChecked.HasValue && ListRadioButton.IsChecked.Value)
            {
                valueType = ValueType.List;
            }
            else if (DictionaryRadioButton.IsChecked.HasValue && DictionaryRadioButton.IsChecked.Value)
            {
                valueType = ValueType.Dictionary;
            }
            else if (HashSetRadioButton.IsChecked.HasValue && HashSetRadioButton.IsChecked.Value)
            {
                valueType = ValueType.HashSet;
            }
            #endregion

            SetStatus(Status.Processing, string.Empty);

            ThreadPool.QueueUserWorkItem(new WaitCallback(item =>
            {
                var returnValue = Process(providerName, key, valueType);

                //var output = string.Format(AppConst.OutputTemplate, value, DateTime.Now);

                this.Dispatcher.Invoke(new Action(() =>
                {
                    OutputTextBox.Text = returnValue;

                    SetStatus(Status.Ready, string.Empty);
                }));
            }), 21);
        }

        private string Process(string providerName, string key, ValueType valueType)
        {
            string returnValue = string.Empty;

            try
            {
                var provider = RedisProvider.GetConnectionMultiplexer(providerName).GetDatabase();

                switch (valueType)
                {
                    case ValueType.String:
                        {
                            returnValue = provider.StringGet(key);
                        }
                        break;
                    case ValueType.List:
                        {
                            var entries = provider.ListRange(key);
                            if (entries != null)
                            {
                                returnValue = JsonConvert.SerializeObject(entries, Formatting.Indented);
                            }
                        }
                        break;
                    case ValueType.Dictionary:
                        {
                            var entries = provider.HashGetAll(key);

                            if (entries != null)
                            {
                                var map = entries.ToDictionary(v => v.Name, v => v.Value);

                                returnValue = JsonConvert.SerializeObject(map, Formatting.Indented);
                            }
                        }
                        break;
                    case ValueType.HashSet:
                        {
                            var hashSetValue = provider.SetMembers(key);

                            if (hashSetValue != null)
                            {
                                returnValue = JsonConvert.SerializeObject(hashSetValue, Formatting.Indented);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                returnValue = ex.ToString();
            }

            if (string.IsNullOrEmpty(returnValue))
            {
                returnValue = "Value not found.";
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