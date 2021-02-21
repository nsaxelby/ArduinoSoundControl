using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ArduinoVolumeLib
{
    public class SerialConnector
    {
        public event EventHandler<SerialStateChangeEventArgs> StateChangeEvent;
        public event EventHandler<CommandFromArduinoEventArgs> CommandReceivedEvent;

        private SerialPort _serialPort;
        private Thread _readThread;
        static bool _continue = true;

        public SerialConnector()
        {
            _serialPort = new SerialPort
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                NewLine = ";"
            };
        }

        public bool Connect()
        {
            if(ConnectToAvailableSocket() == true)
            {
                Console.WriteLine("Connected running read thread");
                _readThread = new Thread(Read);
                _readThread.Start();
                return true;
            }
            return false;
        }

        public void Disconnect()
        {
            try
            {
                _continue = false;
                _readThread.Join();
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to disconnect smoothly: " + ex.Message);
            }
        }

        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    ProcessMessage(message);
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
                catch (Exception) { }
            }
        }

        private void ProcessMessage(string message)
        {
            if (message.StartsWith("UP-1") || message.StartsWith("DOWN-1") || message.StartsWith("PRESS-1"))
            {
                RaiseCommandReceivedEvent(new CommandFromArduinoEventArgs(1, EnumFromString(message)));
            }
            else if (message.StartsWith("UP-2") || message.StartsWith("DOWN-2") || message.StartsWith("PRESS-2"))
            {
                RaiseCommandReceivedEvent(new CommandFromArduinoEventArgs(2, EnumFromString(message)));
            }
            else if (message.StartsWith("UP-3") || message.StartsWith("DOWN-3") || message.StartsWith("PRESS-3"))
            {
                RaiseCommandReceivedEvent(new CommandFromArduinoEventArgs(3, EnumFromString(message)));
            }
            else
            {
                Console.WriteLine("Unknown message: " + message);
            }
        }

        private static SerialCommandsEnum EnumFromString(string cmd)
        {
            if(cmd.StartsWith("UP"))
            {
                return SerialCommandsEnum.UP;
            }
            if(cmd.StartsWith("DOWN"))
            {
                return SerialCommandsEnum.DOWN;
            }
            if(cmd.StartsWith("PRESS"))
            {
                return SerialCommandsEnum.PRESS;
            }
            throw new Exception("Unknown command: " + cmd);
        }

        private bool ConnectToAvailableSocket()
        {
            if (_serialPort.IsOpen == false)
            {
                RaiseStateChangeEvent(new SerialStateChangeEventArgs("Started Connecting via Connect() Method", SerialStateEnum.Connecting, ""));

                if (SerialPort.GetPortNames().Length == 0)
                {
                    RaiseStateChangeEvent(new SerialStateChangeEventArgs("No COM ports found", SerialStateEnum.Error, "ALL"));
                    return false;
                }

                foreach (string portName in SerialPort.GetPortNames())
                {
                    if (TryConnectArduino(portName) == true)
                    {
                        RaiseStateChangeEvent(new SerialStateChangeEventArgs("Connected on Port : " + portName, SerialStateEnum.Connected, portName));
                        break;
                    }
                }

                if (_serialPort.IsOpen == false)
                {
                    RaiseStateChangeEvent(new SerialStateChangeEventArgs("Failed to connect on any COM Ports", SerialStateEnum.Error, "ALL"));
                    return false;
                }
            }
            return true;
        }

        private bool TryConnectArduino(string portName)
        {
            RaiseStateChangeEvent(new SerialStateChangeEventArgs("Trying to Connect to :" + portName, SerialStateEnum.Connecting, portName));
            try
            {
                _serialPort.PortName = portName;
                _serialPort.Open();

                if(_serialPort.IsOpen == false)
                {
                    return false;
                }
                else
                {
                    // Clear the output buffer that the arduino might have ( should a user use the enocders before connecting )
                    _serialPort.ReadExisting();

                    _serialPort.WriteLine("ECHO:Hello");
                    var response = _serialPort.ReadLine();

                    Console.WriteLine("Response: " + response);
                    if (response == "ECHO:Hello")
                    {
                        return true;
                    }
                    else
                    {
                        if(_serialPort.IsOpen)
                        {
                            _serialPort.Close();
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                Console.WriteLine("Didn't connect to: " + portName + " - " + ex.Message);
                return false;
            }
        }

        protected virtual void RaiseStateChangeEvent(SerialStateChangeEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<SerialStateChangeEventArgs> raiseEvent = StateChangeEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        protected virtual void RaiseCommandReceivedEvent(CommandFromArduinoEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<CommandFromArduinoEventArgs> raiseEvent = CommandReceivedEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        public void SendMVolChange(String deviceName, float inputVol)
        {
            if (_serialPort.IsOpen)
            {
                deviceName = Util.NormalizeNameForRow(deviceName);
                _serialPort.Write("ROW1:" + deviceName + ";");
                string row2line = "ROW2:" + Util.VolumeToRow2Bars(inputVol * 100) + ";";
                Console.WriteLine(row2line);
                _serialPort.Write(row2line);
            }
        }

        public void SendMutedChange(String deviceName)
        {
            if (_serialPort.IsOpen)
            {
                deviceName = Util.NormalizeNameForRow(deviceName);
                _serialPort.Write("ROW1:" + deviceName + ";");
                string row2line = "ROW2:" + "-----MUTED------;";
                Console.WriteLine(row2line);
                _serialPort.Write(row2line);
            }
        }

        public bool IsConnected()
        {
            return _serialPort.IsOpen;
        }
    }
}
