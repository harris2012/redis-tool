using StackExchange.Redis;
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
    /// Interaction logic for WritePage.xaml
    /// </summary>
    public partial class WritePage : UserControl
    {
        public WritePage()
        {
            InitializeComponent();
        }

        private void WriteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(ProviderTextBox.Text)
                && !string.IsNullOrEmpty(KeyTextBox.Text)
                && (DictionaryRadioButton.IsChecked == null || !DictionaryRadioButton.IsChecked.Value || !string.IsNullOrEmpty(SubKeyTextBox.Text))
                && !string.IsNullOrEmpty(ValueTextBox.Text);
        }

        private void WriteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ComfirmRequiredCheckBox.IsChecked != null
                && ComfirmRequiredCheckBox.IsChecked.Value
                && MessageBox.Show("Are you sure to write?", "Comfirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

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
            else if (JsonRadioButton.IsChecked.HasValue && JsonRadioButton.IsChecked.Value)
            {
                valueType = ValueType.Json;
            }
            #endregion

            string subKey = SubKeyTextBox.Text;

            string value = ValueTextBox.Text;

            SetStatus(Status.Processing, string.Empty);

            ThreadPool.QueueUserWorkItem(new WaitCallback(item =>
            {
                var returnValue = Process(providerName, key, valueType, subKey, value);

                //var output = string.Format(AppConst.OutputTemplate, returnValue, DateTime.Now);

                this.Dispatcher.Invoke(new Action(() =>
                {
                    //OutputTextBox.Text = returnValue;

                    SetStatus(Status.Ready, returnValue);
                }));
            }), 21);
        }

        private string Process(string providerName, string key, ValueType valueType, string subKey, string value)
        {
            string returnValue = string.Empty;

            try
            {
                bool writeResult = false;

                var provider = RedisProvider.GetConnectionMultiplexer(providerName).GetDatabase();

                switch (valueType)
                {
                    case ValueType.String:
                        {
                            writeResult = provider.StringSet(key, value);
                        }
                        break;
                    case ValueType.List:
                        {
                            writeResult = provider.ListRightPush(key, value) > 0;
                        }
                        break;
                    case ValueType.Dictionary:
                        {
                            writeResult = provider.HashSet(key, subKey, value);
                        }
                        break;
                    case ValueType.HashSet:
                        {
                            writeResult = provider.SetAdd(key, value);
                        }
                        break;
                    case ValueType.Json:
                        {
                            //var items = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                            //if (items != null && items.Count > 0)
                            //{
                            //    writeResult = true;
                            //    foreach (var item in items)
                            //    {
                            //        provider.SetEntryInHash(key, item.Key, item.Value);
                            //        if(!writeResult)
                            //        {
                            //            break;
                            //        }
                            //    }
                            //}
                        }
                        break;
                    default:
                        break;
                }

                if (writeResult)
                {
                    returnValue = "Redis write successfully.";
                }
                else
                {
                    returnValue = "Redis write failed.";
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
