using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS_232
{
    public class ConnectionService
    {
        private Stopwatch stopWatch = new Stopwatch();
        private SerialPort serialPort;

        public List<string> GetPortNames()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public bool ConfigurePort(string portName, int baudRate, string charFormat, string terminator, FlowControl flowControl)
        {
            serialPort = new SerialPort();

            if (flowControl.Equals(FlowControl.RTS_CTS))
            {
                flowControl = FlowControl.DTR_DSR;
            }

            serialPort.PortName = portName;
            serialPort.DataBits = int.Parse(charFormat[0].ToString());
            serialPort.BaudRate = baudRate;
            serialPort.Parity = GetParityFromCharFormat(charFormat);
            serialPort.Handshake = (Handshake)flowControl;
            serialPort.StopBits = (StopBits)int.Parse(charFormat[2].ToString());

            serialPort.NewLine = terminator;

            try
            {
                serialPort.Open();

                return true;
            }
            catch(Exception ex)
            {
                serialPort.Close();

                return false;
            }
        }

        public void Close()
        {
            if(serialPort != null)
            {
                serialPort.Close();
            }
        }

        public string SendMessage(string message)
        {
            if (!serialPort.IsOpen)
            {
                return "Error writing to the port";
            }
            else
            {
                serialPort.WriteLine(message);
                return "Message sent";
            }
        }

        public string ParseMessage(string message)
        {
            if (message.Equals("PING"))
            {
                SendMessage("PONG");
            }
            else if (message.Equals("PONG") && stopWatch.IsRunning)
            {
                stopWatch.Stop();

                return $"PONG {stopWatch.Elapsed.TotalMilliseconds} ms";
            }

            return message;
        }

        public void SetDataReceivedHandler(SerialDataReceivedEventHandler serialDataReceivedEventHandler)
        {
            serialPort.DataReceived += serialDataReceivedEventHandler;
        }

        public void Ping()
        {
            SendMessage("PING");
            stopWatch.Restart();
        }

        public static Parity GetParityFromCharFormat(string charFormat)
        {
            switch (charFormat[1])
            {
                case 'O':
                    return Parity.Odd;
                case 'E':
                    return Parity.Even;
                case 'N':
                default:
                    return Parity.None;
            }
        }

        public static string GetStringFromTerminator(Terminator ter)
        {
            switch (ter)
            {
                case Terminator.CR:
                    return "\r";
                case Terminator.LF:
                    return "\n";
                case Terminator.CRLF:
                    return "\r\n";
                case Terminator.BRAK:
                case Terminator.CUSTOM:
                default:
                    return " ";
            }
        }
    }

    public enum Terminator { CR, LF, CRLF, BRAK, CUSTOM };

    public enum FlowControl { BRAK, XON_XOFF, RTS_CTS, DTR_DSR };
}
