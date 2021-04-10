using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoVolumeLib
{
    public class DeviceListResponseEventArgs : EventArgs
    {
        public List<DeviceItem> DeviceItems { get; set; }
        public DeviceListResponseEventArgs(List<DeviceItem> devs)
        {
            DeviceItems = devs;
        }
    }

    public class DeviceItem
    {
        public String Name { get; set; }
        public string DeviceID { get; set; }
        public List<int> EncoderNumbers { get; set; }
        public List<SoundSessionItem> SoundSessions { get; set; }
        public float CurrentVolume { get; set; }
        public bool CurrentMuted { get; set; }

        public DeviceItem(string devName, string devID, List<int> encoderNumbers, float currentVol, bool currentMuted)
        {
            this.Name = devName;
            this.DeviceID = devID;
            this.EncoderNumbers = encoderNumbers;
            SoundSessions = new List<SoundSessionItem>();
            CurrentVolume = currentVol;
            CurrentMuted = currentMuted;
        }
    }

    public class SoundSessionItem
    {
        public string Name { get; set; }
        public uint SoundSessionProcessID { get; set; }
        public List<int> EncoderNumbers { get; set; }
        public float CurrentVolume { get; set; }
        public bool CurrentMuted { get; set; }

        public SoundSessionItem(string sessionName, uint SoundSessionProcessID, List<int> encoderNumbers, float currentVolume, bool currentMuted)
        {
            this.Name = sessionName;
            this.SoundSessionProcessID = SoundSessionProcessID;
            this.EncoderNumbers = encoderNumbers;
            CurrentVolume = currentVolume;
            CurrentMuted = currentMuted;
        }
    }

}
