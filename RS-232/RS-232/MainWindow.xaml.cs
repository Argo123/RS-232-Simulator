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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS_232
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ConnectionService connectionService;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConfigureButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ConfigureWindow();

            window.Show();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = TextBox.Text;
            connectionService.SendMessage(message);

            string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            RichTextBox.AppendText(time + " Sent: " + message);
            RichTextBox.AppendText(Environment.NewLine);
            TextBox.Text = string.Empty;
        }

        private void PingButton_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            RichTextBox.AppendText(time + " Pinged!");
            RichTextBox.AppendText(Environment.NewLine);

            connectionService.Ping();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextBox.Selection.Text = "";
        }

        private void modbusButton_Click(object sender, RoutedEventArgs e)
        {
            var modbusWindow = new ModbusWindow();
            App.Current.MainWindow = modbusWindow;

            modbusWindow.Show();
            this.Close();
        }
    }
}
