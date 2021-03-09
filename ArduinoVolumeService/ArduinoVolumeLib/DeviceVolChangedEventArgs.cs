using System;
using System.Collections.Generic;
using System.Linq;

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

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                DeviceVolChangedEventArgs newObj = (DeviceVolChangedEventArgs)obj;
                if(newObj.Name.Equals(Name) &&
                    newObj.Volume.Equals(Volume) &&
                    newObj.Muted.Equals(Muted) &&
                    newObj.EncoderNumbers.SequenceEqual(EncoderNumbers))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
