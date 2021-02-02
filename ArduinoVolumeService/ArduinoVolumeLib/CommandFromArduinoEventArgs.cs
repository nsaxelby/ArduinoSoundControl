using System;

namespace ArduinoVolumeLib
{
    public class CommandFromArduinoEventArgs : EventArgs
    {
        public int EncoderNumber { get; set; }
        public CommandsEnum Command { get; set; }

        public CommandFromArduinoEventArgs(int encoderNumber, CommandsEnum command)
        {
            EncoderNumber = encoderNumber;
            Command = command;
        }
    }
}
