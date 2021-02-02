using System;
using System.IO.Ports;
using System.Threading;

namespace ArduinoVolumeLib
{
    public class SerialConnector
    {
        public event EventHandler<StateChangeEventArgs> StateChangeEvent;
        public event EventHandler<CommandFromArduinoEventArgs> CommandReceivedEvent;
        static SerialPort _serialPort;
        Thread _readThread;
        static bool _continue;

        public SerialConnector()
        {
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.NewLine = ";";
        }

        public bool Connect()
        {
            if(ConnectToAvailableSocket() == true)
            {
                _readThread = new Thread(Read);
                _readThread.Start();
                return true;
            }
            return false;
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

        private static CommandsEnum EnumFromString(string cmd)
        {
            if(cmd.StartsWith("UP"))
            {
                return CommandsEnum.UP;
            }
            if(cmd.StartsWith("DOWN"))
            {
                return CommandsEnum.DOWN;
            }
            if(cmd.StartsWith("PRESS"))
            {
                return CommandsEnum.PRESS;
            }
            throw new Exception("Unknown command: " + cmd);
        }

        private bool ConnectToAvailableSocket()
        {
            if (_serialPort.IsOpen == false)
            {
                RaiseStateChangeEvent(new StateChangeEventArgs("Started Connecting via Connect() Method", SerialStateEnum.Connecting, ""));

                if (SerialPort.GetPortNames().Length == 0)
                {
                    RaiseStateChangeEvent(new StateChangeEventArgs("No COM ports found", SerialStateEnum.Error, "ALL"));
                    return false;
                }

                foreach (string portName in SerialPort.GetPortNames())
                {
                    if (TryConnectArduino(portName) == true)
                    {
                        RaiseStateChangeEvent(new StateChangeEventArgs("Connected on Port : " + portName, SerialStateEnum.Connected, portName));
                        break;
                    }
                }

                if (_serialPort.IsOpen)
                {
                    RaiseStateChangeEvent(new StateChangeEventArgs("Failed to connect on any COM Ports", SerialStateEnum.Error, "ALL"));
                    return false;
                }
            }
            return true;
        }

        private bool TryConnectArduino(string portName)
        {
            RaiseStateChangeEvent(new StateChangeEventArgs("Trying to Connect to :" + portName, SerialStateEnum.Connecting, portName));
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
                    _serialPort.WriteLine("ECHO:Hello");
                    if(_serialPort.ReadLine() == "ECHO:Hello")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Didn't connect to: " + portName + " - " + ex.Message);
                return false;
            }
        }

        protected virtual void RaiseStateChangeEvent(StateChangeEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<StateChangeEventArgs> raiseEvent = StateChangeEvent;

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
    }
}
