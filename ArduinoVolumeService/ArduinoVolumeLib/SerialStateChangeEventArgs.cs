using System;

namespace ArduinoVolumeLib
{
    public class SerialStateChangeEventArgs : EventArgs
    {
        public SerialStateEnum State { get; set; }
        public string Message { get; set; }
        public string PortName { get; set; }
        public SerialStateChangeEventArgs(string message, SerialStateEnum state, string portName)
        {
            State = state;
            Message = message;
            PortName = portName;
        }
    }
}
