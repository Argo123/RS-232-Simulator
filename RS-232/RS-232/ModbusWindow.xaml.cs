using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RS_232
{
    /// <summary>
    /// Interaction logic for ModbusWindow.xaml
    /// </summary>
    public partial class ModbusWindow : Window
    {
        public ModbusWindow()
        {
            this.Port = new Core.SerialPortWrapper();
            this.LogItems = new ObservableCollection<LogItemTemplate>();
            this.TextInput = "Hello RS-232";
            this.Port.DataReceived += Port_DataReceived;
            this.Port.ErrorReceived += Port_ErrorReceived;
            this.Port.PinChanged += Port_PinChanged;
            this.Title = "RS-232";
            this.IsSlave = true;
            this.TimeBetweenCharactersTimeout = 1000;
            this.SlaveAddress = 0;
            this.SlaveToSend = 0;
            this.AmountOfRetransmision = 1;
        }


        #region MODBUS

        private int _amountOfRetransmison;

        public int AmountOfRetransmision
        {
            get { return _amountOfRetransmison; }
            set
            {
                _amountOfRetransmison = value;
                this.Notify("AmountOfRetransmision");
            }
        }

        private bool _isSlave;

        public bool IsSlave
        {
            get { return _isSlave; }
            set
            {
                _isSlave = value;
                this.Notify("IsSlave");
            }
        }


        private short _slaveAddress;

        public short SlaveAddress
        {
            get { return _slaveAddress; }
            set
            {
                _slaveAddress = value;
                this.Notify("SlaveAddress");
            }
        }

        private short _slaveToSend;

        public short SlaveToSend
        {
            get { return _slaveToSend; }
            set
            {
                _slaveToSend = value;
                this.Notify("SlaveToSend");
            }
        }

        private int _timeBetweenCharactersTimeout;

        public int TimeBetweenCharactersTimeout
        {
            get { return _timeBetweenCharactersTimeout; }
            set
            {
                _timeBetweenCharactersTimeout = value;
                this.Notify("TimeBetweenCharactersTimeout");
            }
        }

        #endregion

        #region Properties & Fields

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                this.Notify("Title");
            }
        }

        private ObservableCollection<LogItemTemplate> _logItems;

        public ObservableCollection<LogItemTemplate> LogItems
        {
            get { return _logItems; }
            set
            {
                _logItems = value;
                this.Notify("LogItems");
            }
        }

        private string _textInput;

        public string TextInput
        {
            get { return _textInput; }
            set
            {
                _textInput = value;
                this.Notify("TextInput");
            }
        }

        private bool IsConfigured;

        #region Workers
        private BackgroundWorker readWorker;
        private BackgroundWorker writeWorker;
        #endregion

        #endregion

        #region Model
        private Core.SerialPortWrapper _port;

        public Core.SerialPortWrapper Port
        {
            get { return _port; }
            set
            {
                _port = value;
                this.Notify("Port");
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Commands

        #region Open
        ICommand _openPort;
        public ICommand OpenPort
        {
            get
            {
                return _openPort ??
                    (_openPort = new RelayCommand(OpenPortClick));
            }
        }

        private void OpenPortClick(object obj)
        {
            if (this.Port.IsOpen || this.IsConfigured)
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Error, "Port is already used!"));
            }
            else
            {
                this.IsConfigured = true;
                this.Port.Open();
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Just Opened port " + Port.Name));
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Started listening"));
                this.Title = "RS-232" + ":" + this.Port.Name;
            }
            this.Notify("Port");
        }

        #endregion

        #region RefreshLines
        ICommand _refreshLines;
        public ICommand RefreshLines
        {
            get
            {
                return _refreshLines ??
                    (_refreshLines = new RelayCommand(RefreshLinesClick));
            }
        }

        private void RefreshLinesClick(object obj)
        {
            this.Notify("Port");
        }
        #endregion

        #region WriteTransact
        ICommand _writeTransactMsg;
        public ICommand WriteTransact
        {
            get
            {
                return _writeTransactMsg ??
                    (_writeTransactMsg = new RelayCommand(WriteTransactClick));
            }
        }

        private void WriteTransactClick(object obj)
        {
            if (!this.IsSlave)
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingText, string.Format("\"{0}\"\n to {1}", this.TextInput, this.SlaveToSend)));

                var srttmp = string.Empty;
                new Core.ModubusASCIIFrame(this.SlaveToSend, 1, this.TextInput).GetFrame().ToList().ForEach(x => srttmp += x.ToString() + ";");
                App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingHex, string.Format("\"{0}\"", srttmp)))));

                this.Port.ModbusTransaction(new Core.ModubusASCIIFrame(this.SlaveToSend, 1, this.TextInput).GetFrame(), this.AmountOfRetransmision);

            }
        }

        #endregion

        #region WriteAll

        ICommand _writeAll;
        public ICommand WriteAll
        {
            get
            {
                return _writeAll ??
                    (_writeAll = new RelayCommand(WriteAllClick));
            }
        }

        private void WriteAllClick(object obj)
        {
            if (!this.IsSlave)
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingText, string.Format("\"{0}\"\n to {1}", this.TextInput, this.SlaveToSend)));

                this.Port.ModbusWrite(new Core.ModubusASCIIFrame(0xFF, 1, this.TextInput).GetFrame());

                var srttmp = string.Empty;
                new Core.ModubusASCIIFrame(0xFF, 1, this.TextInput).GetFrame().ToList().ForEach(x => srttmp += x.ToString() + ";");
                App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingHex, string.Format("\"{0}\"", srttmp)))));


            }
        }

        #endregion

        #region RetriveFrom

        private ICommand _retriveAll;

        public ICommand RetriveFrom
        {
            get
            {
                return _retriveAll ??
                    (_retriveAll = new RelayCommand(RetriveAllClick));
            }
        }

        private void RetriveAllClick(object obj)
        {
            if (!this.IsSlave)
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingText, string.Format("Give me string from slave\n{0}", this.SlaveToSend)));

                this.Port.ModbusTransaction(new Core.ModubusASCIIFrame(this.SlaveToSend, 2, "").GetFrame(), this.AmountOfRetransmision);
            }
        }

        #endregion

        #region ClosePort

        ICommand _closePort;
        public ICommand ClosePort
        {
            get
            {
                return _closePort ??
                    (_closePort = new RelayCommand(ClosePortClick));
            }
        }

        private void ClosePortClick(object obj)
        {
            if (this.Port.IsOpen && this.IsConfigured
                || !this.Port.IsOpen && this.IsConfigured)
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Closed port!"));
                this.Port.Close();
                this.IsConfigured = false;
                this.Title = "RS-232";
            }
            else if (!this.IsConfigured && !this.Port.IsOpen)
            {

            }
            else
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Error, "Cannot access to port: " + Port.Name));
            }
            this.Notify("Port");
        }
        #endregion

        #region Reconfigure
        ICommand _reconfigurePort;
        public ICommand Reconfigure
        {
            get
            {
                return _reconfigurePort ??
                    (_reconfigurePort = new RelayCommand(ReconfigureClick));
            }
        }

        private void ReconfigureClick(object obj)
        {
            if (this.Port.IsOpen && this.IsConfigured
                || !this.Port.IsOpen && this.IsConfigured)
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Reconfigured port!"));
                this.Port.Close();
                this.Port.Open();
                this.IsConfigured = true;
                this.Title = "RS-232" + ":" + this.Port.Name;
            }
            else if (!this.IsConfigured && !this.Port.IsOpen)
            {
                this.OpenPortClick(null);
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Reconfigured port!"));
            }
            else
            {
                this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Cannot access to port: " + Port.Name));
            }
            this.Notify("Port");
        }
        #endregion


        #endregion

        #region Events
        #endregion

        #region Event handlers
        byte[] tmp = new byte[] { 24 };
        private void Port_DataReceived(string msg, bool isBuffer)
        {
            if (isBuffer)
            {
                App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Buffer, string.Format("\"{0}\"", msg)))));
            }
            else
            {
                App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Receiving, string.Format("\"{0}\"", msg)))));

                var srtrtmp = string.Empty;
                Encoding.ASCII.GetBytes(msg).ToList().ForEach(x => srtrtmp += x.ToString() + ";");
                App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Receiving, string.Format("\"{0}\"", srtrtmp)))));
                var frame = new Core.ModubusASCIIFrame(ASCIIEncoding.ASCII.GetBytes(msg));

                App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Receiving, Encoding.ASCII.GetString(frame.Data)))));


                if (frame.IsValid() && (this.SlaveAddress == frame.Address || frame.Address == 0xFF))
                {
                    App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Information, "Frame ok!"))));
                    if (frame.FunctionCode == 1 && this.SlaveAddress == frame.Address)
                    {
                        tmp = frame.Data;
                        this.Port.ModbusWrite(new Core.ModubusASCIIFrame(0, 0, "ACK").GetFrame());
                        var srttmp = string.Empty;
                        new Core.ModubusASCIIFrame(0, 0, "ACK").GetFrame().ToList().ForEach(x => srttmp += x.ToString() + ";");
                        App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingHex, string.Format("\"{0}\"", srttmp)))));

                    }
                    else if (frame.FunctionCode == 2)
                    {
                        var srttmp = string.Empty;
                        new Core.ModubusASCIIFrame(0, 0, Encoding.ASCII.GetString(tmp)).GetFrame().ToList().ForEach(x => srttmp += x.ToString() + ";");
                        App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.SendingHex, string.Format("\"{0}\"", srttmp)))));

                        this.Port.ModbusWrite(new Core.ModubusASCIIFrame(0, 0, Encoding.ASCII.GetString(tmp)).GetFrame());
                    }
                }
                else
                {
                    App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Error, "Frame error!"))));
                }


            }
        }

        private void Port_PinChanged(string msg)
        {
            App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.PinChanged, string.Format("\"{0}\"", msg)))));
            this.Notify("Port");
        }

        private void Port_ErrorReceived(string msg)
        {
            App.Current.Dispatcher.Invoke(new Action(() => this.LogItems.Add(new LogItemTemplate(LogTypesEnum.Error, string.Format("\"{0}\"", msg)))));
            this.Notify("Port");
        }
        #endregion

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
