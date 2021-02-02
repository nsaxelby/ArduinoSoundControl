using System;

namespace ArduinoVolumeLib
{
    public class StateChangeEventArgs : EventArgs
    {
        public SerialStateEnum State { get; set; }
        public string Message { get; set; }
        public string PortName { get; set; }
        public StateChangeEventArgs(string message, SerialStateEnum state, string portName)
        {
            State = state;
            Message = message;
            PortName = portName;
        }
    }
}
