using System;

namespace ArduinoVolumeLib
{
    class VolChangeCommandFromWebEventArgs : EventArgs
    {
        public int EncoderNumber { get; set; }
        public int Volume { get; set; }
        public bool Muted { get; set; }

        public VolChangeCommandFromWebEventArgs(int encoderNumber, int volume, bool muted)
        {
            this.EncoderNumber = encoderNumber;
            this.Volume = volume;
            this.Muted = muted;
        }
    }
}
