using System;
using System.Collections.Generic;

namespace ArduinoVolumeLib
{
    public class DeviceVolChangedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public List<int> EncoderNumbers { get; set; }
        public float Volume { get; set; }
        public bool Muted { get; set; }

        public DeviceVolChangedEventArgs(string name, float volume, bool muted, List<int> enocderNumbers)
        {
            this.Name = name;
            this.Volume = volume;
            this.Muted = muted;
            this.EncoderNumbers = enocderNumbers;
        }
    }
}
