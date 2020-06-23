using System.Collections.Generic;
using System.IO.Ports;
using System;
using System.Windows;
using EasyModbus;
using System.Windows.Controls;

namespace RS_232
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    public partial class ConfigureModbusWindow : Window
    {
        public ModbusClient modbusClient = new ModbusClient();
        public ModbusServer modbusServer = new ModbusServer();
        public ConnectionService connectionService = new ConnectionService();
        
        public ConfigureModbusWindow()
        {
            InitializeComponent();

            PortComboBox.ItemsSource = connectionService.GetPortNames();
            SpeedComboBox.ItemsSource = new List<int> { 150, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200 };
            FormatComboBox.ItemsSource = new List<string> { "8E1", "8O1", "8N2", "7E1", "7O1", "7N2" };
            RetransmissionComboBox.ItemsSource = new List<string> { "0", "1", "2", "3", "4", "5" };
            adressComboBox.ItemsSource = new List<string> { "1", "2", "3", "4", "5" }; 
            StationTypeComboBox.ItemsSource = new List<string> { "MASTER", "SLAVE" };
        }

        private void ConfigureButton_Click(object sender, RoutedEventArgs e)
        {
            int baudRate = int.Parse(SpeedComboBox.Text);
            string charFormat = FormatComboBox.Text;
            var flowControl = FlowControl.BRAK;
            string portName = PortComboBox.Text;

            if (string.IsNullOrEmpty(portName))
            {
                MessageBox.Show("Please choose a port.");

                return;
            }

            var success = connectionService.ConfigurePort(portName, baudRate, charFormat, "CR", flowControl);

            var mainWindow = App.Current.MainWindow as ModbusWindow;
            var richTextBox = mainWindow.FindName("richTextBox") as RichTextBox;

            if (success)
            {

                richTextBox.AppendText("Port configurated successfuly!");
                richTextBox.AppendText(Environment.NewLine);

                mainWindow.connectionService = connectionService;
            }
            else
            {
                richTextBox.AppendText("Port configuration failure!");
                richTextBox.AppendText(Environment.NewLine);
            }

            connectionService.SetDataReceivedHandler(new SerialDataReceivedEventHandler(DataReceivedHandler));

            if(StationTypeComboBox.Text == "MASTER")
            {
                ConfigureMaster();
            }
            else
            {
                ConfigureSlave();
            }

            this.Close();
        }
        private void ConfigureMaster()
        {
            connectionService.slaveAddress = -1;

            var mainWindow = App.Current.MainWindow as ModbusWindow;
            mainWindow.ConfigureMaster();
            //mainWindow.
        }
        private void ConfigureSlave()
        {
            connectionService.slaveAddress = Int32.Parse(adressComboBox.Text);

            var mainWindow = App.Current.MainWindow as ModbusWindow;
            mainWindow.ConfigureSlave();
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
                    if (message[1].ToString() == connectionService.slaveAddress.ToString())
                    {
                        message = message.Substring(0, message.Length - sp.NewLine.Length);
                        message = connectionService.ParseMessage(message);
                        string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                        var mainWindow = App.Current.MainWindow;
                        var richTextBox = mainWindow.FindName("richTextBox") as RichTextBox;

                        var operationTextBox = mainWindow.FindName("sendtextBox") as TextBox;
                        var argumentTextBox = mainWindow.FindName("argumentTextBox") as TextBox;

                        operationTextBox.Text = message.Substring(2, message.IndexOf('.') - 2);
                        argumentTextBox.Text = message.Substring(message.IndexOf('.') + 1, message.IndexOf(',') - message.IndexOf('.') - 1);

                        richTextBox.AppendText(time + " Recieved frame!");
                        richTextBox.AppendText(Environment.NewLine);

                        ReplyMaster(operationTextBox.Text + " " + argumentTextBox.Text + " from address " + connectionService.slaveAddress.ToString());
                    }
                    else
                    if(message[1] == '-' && connectionService.slaveAddress == -1)
                    {
                        message = message.Substring(0, message.Length - sp.NewLine.Length);
                        message = connectionService.ParseMessage(message);
                        string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

                        var mainWindow = App.Current.MainWindow;
                        var richTextBox = mainWindow.FindName("richTextBox") as RichTextBox;

                        richTextBox.AppendText(time + " Recieved - " + message);
                        richTextBox.AppendText(Environment.NewLine);
                    }
                });
            }
        }

        private void ReplyMaster(string message)
        {
            var frame = new MODBUSFrame(-1, "REPLY: ", message, "CR");

            connectionService.SendMessage(frame.MakeFrame());
        }

        private void StationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(StationTypeComboBox.Text == "SLAVE")
            {
                adressComboBox.IsEnabled = false;
            }
            else
            {
                adressComboBox.IsEnabled = true;
            }
        }
    }
}

