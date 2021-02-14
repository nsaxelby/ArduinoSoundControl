using System;

namespace ArduinoVolumeLib
{
    public class CommandFromArduinoEventArgs : EventArgs
    {
        // Encoder number is numerical starting at 1
        public int EncoderNumber { get; set; }
        public SerialCommandsEnum Command { get; set; }

        public CommandFromArduinoEventArgs(int encoderNumber, SerialCommandsEnum command)
        {
            EncoderNumber = encoderNumber;
            Command = command;
        }
    }
}
