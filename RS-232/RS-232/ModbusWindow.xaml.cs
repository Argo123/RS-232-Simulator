using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EasyModbus;

namespace RS_232
{
    /// <summary>
    /// Interaction logic for ModbusWindow.xaml
    /// </summary>
    public partial class ModbusWindow : Window
    {
        public bool isMaster;
        public ModbusServer modbusServer;
        public ModbusClient modbusClient;
        public ConnectionService connectionService;
        public ModbusWindow()
        {
            InitializeComponent();
        }

        public void ConfigureSlave()
        {
            InitializeComponent();
            configureButton.IsEnabled = false;
            clearButton.IsEnabled = true;

            comboBox.ItemsSource = new List<int> { 1, 2, 3, 4, 5 };
            comboBox.SelectedItem = connectionService.slaveAddress;
        }

        public void ConfigureMaster()
        {
            InitializeComponent();
            configureButton.IsEnabled = false;
            sendButton.IsEnabled = true;
            sendtextBox.IsEnabled = true;
            richTextBox.IsEnabled = true;
            clearButton.IsEnabled = true;
            argumentTextBox.IsEnabled = true;
            broadcastButton.IsEnabled = true;

            comboBox.IsEnabled = true;
            comboBox.ItemsSource = new List<int> { 1, 2, 3, 4, 5 };
        }

        private void configureButton_Click(object sender, RoutedEventArgs e)
        {
            var configureModbusWindow = new ConfigureModbusWindow();

            configureModbusWindow.Show();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = new MODBUSFrame(Int32.Parse(comboBox.Text), sendtextBox.Text, argumentTextBox.Text, "CR");
            string message = frame.MakeFrame();

            connectionService.SendMessage(message);

            string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            richTextBox.AppendText(time + " Sent OP: " + sendtextBox.Text + " ARG: " + argumentTextBox.Text + " to: " + comboBox.Text);
            richTextBox.AppendText(Environment.NewLine);
            sendtextBox.Text = string.Empty;
        }

        private void broadcastButton_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 1; i <= 5; i++)
            {
                var frame = new MODBUSFrame(i, sendtextBox.Text, argumentTextBox.Text, "CR");
                string message = frame.MakeFrame();

                connectionService.SendMessage(message);

                string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                richTextBox.AppendText(time + " Sent OP: " + sendtextBox.Text + " ARG: " + argumentTextBox.Text + " to: " + i.ToString());
                richTextBox.AppendText(Environment.NewLine);
                sendtextBox.Text = string.Empty;
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            //richTextBox = new RichTextBox();
        }
    }
}
