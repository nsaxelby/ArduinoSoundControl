using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoVolumeLib
{
    public class DeviceListChangedEventArgs : EventArgs
    {
        public List<DeviceItem> DeviceItems { get; set; }
        public DeviceListChangedEventArgs(List<DeviceItem> devs)
        {
            DeviceItems = devs;
        }
    }

    public class DeviceItem
    {
        public String Name { get; set; }
        public string DeviceID { get; set; }
        public int? EncoderNumber { get; set; }
        public List<SoundSessionItem> SoundSessions { get; set; }

        public DeviceItem(string devName, string devID, int? encNumber)
        {
            this.Name = devName;
            this.DeviceID = devID;
            this.EncoderNumber = encNumber;
            SoundSessions = new List<SoundSessionItem>();
        }
    }

    public class SoundSessionItem
    {
        public string Name { get; set; }
        public string SoundSessionID { get; set; }
        public int? EncoderNumber { get; set; }

        public SoundSessionItem(string sessionName, string sessionID, int? encoderNumber)
        {
            this.Name = sessionName;
            this.SoundSessionID = sessionID;
            this.EncoderNumber = encoderNumber;
        }
    }

}
