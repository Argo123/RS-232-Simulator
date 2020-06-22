using System.Collections.Generic;
using System.IO.Ports;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RS_232
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    public partial class ConfigureWindow : Window
    {
        public ConnectionService connectionService = new ConnectionService();
        public ConfigureWindow()
        {
            InitializeComponent();

            PortComboBox.ItemsSource = connectionService.GetPortNames();
            SpeedComboBox.ItemsSource = new List<int> { 150, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200 };
            FormatComboBox.ItemsSource = new List<string> { "8E1", "8O1", "8N2", "7E1", "7O1", "7N2" };
            TerminatorComboBox.ItemsSource = Enum.GetNames(typeof(Terminator));
            HandshakeComboBox.ItemsSource = Enum.GetNames(typeof(FlowControl));
        }

        private void TerminatorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TerminatorComboBox.SelectedItem.ToString() == "CUSTOM")
            {
                TerminatorTextBox.IsEnabled = true;
            }
            else
            {
                TerminatorTextBox.IsEnabled = false;
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string message = sp.ReadExisting();

            if (!string.IsNullOrEmpty(message))
            {
                //idk what is does but fixes thread exception
                Dispatcher.Invoke(() => 
                {
                    string terminator = GetTerminatorAsString();
                    message = message.Substring(0, message.Length - sp.NewLine.Length);
                    message = connectionService.ParseMessage(message);
                    string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                    var mainWindow = App.Current.MainWindow;
                    var richTextBox = mainWindow.FindName("RichTextBox") as RichTextBox;

                    richTextBox.AppendText(time + " Recieved: " + message);
                    richTextBox.AppendText(Environment.NewLine);
                });
            }
        }

        private void ConfigureButton_Click(object sender, RoutedEventArgs e)
        {
            int baudRate = int.Parse(SpeedComboBox.Text);
            string charFormat = FormatComboBox.Text;
            string terminator = GetTerminatorAsString();
            var flowControl = (FlowControl)Enum.Parse(typeof(FlowControl), HandshakeComboBox.Text);
            string portName = PortComboBox.Text;

            if (string.IsNullOrEmpty(portName))
            {
                MessageBox.Show("Please choose a port.");

                return;
            }

            var success = connectionService.ConfigurePort(portName, baudRate, charFormat, terminator, flowControl);

            var mainWindow = App.Current.MainWindow as MainWindow;
            var richTextBox = mainWindow.FindName("RichTextBox") as RichTextBox;

            if (success)
            {
                var clearButton = mainWindow.FindName("ClearButton") as Button;
                var sendButton = mainWindow.FindName("SendButton") as Button;
                var pingButton = mainWindow.FindName("PingButton") as Button;
                var sendTextBox = mainWindow.FindName("TextBox") as TextBox;
                var configButton = mainWindow.FindName("ConfigureButton") as Button;

                richTextBox.AppendText("Port configurated successfuly!");
                richTextBox.AppendText(Environment.NewLine);

                sendButton.IsEnabled = true;
                clearButton.IsEnabled = true;
                pingButton.IsEnabled = true;
                sendTextBox.IsEnabled = true;
                configButton.IsEnabled = false;

                mainWindow.connectionService = connectionService;
            }
            else
            {
                richTextBox.AppendText("Port configuration failure!");
                richTextBox.AppendText(Environment.NewLine);
            }

            connectionService.SetDataReceivedHandler(new SerialDataReceivedEventHandler(DataReceivedHandler));
            this.Close();
        }

        private string GetTerminatorAsString()
        {
            if (TerminatorComboBox.SelectedItem.ToString() == "CUSTOM")
            {
                return TerminatorTextBox.Text;
            }

            return TerminatorComboBox.Text;
        }
    }
}
