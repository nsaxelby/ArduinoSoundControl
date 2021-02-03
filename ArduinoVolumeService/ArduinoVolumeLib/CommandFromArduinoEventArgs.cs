using System;

namespace ArduinoVolumeLib
{
    public class CommandFromArduinoEventArgs : EventArgs
    {
        // Encoder number is numerical starting at 1
        public int EncoderNumber { get; set; }
        public CommandsEnum Command { get; set; }

        public CommandFromArduinoEventArgs(int encoderNumber, CommandsEnum command)
        {
            EncoderNumber = encoderNumber;
            Command = command;
        }
    }
}
