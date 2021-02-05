using System;

namespace ArduinoVolumeLib
{
    public class DeviceVolChangedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public float Volume { get; set; }
        public bool Muted { get; set; }

        public DeviceVolChangedEventArgs(string name, float volume, bool muted)
        {
            this.Name = name;
            this.Volume = volume;
            this.Muted = muted;
        }
    }
}
