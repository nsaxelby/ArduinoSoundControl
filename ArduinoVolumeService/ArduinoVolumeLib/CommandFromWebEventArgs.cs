namespace ArduinoVolumeLib
{
    public class CommandFromWebEventArgs
    {
        public int EncoderNumber { get; set; }
        public WebCommandEnum WebCommand { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public bool IsDeviceBinding { get; set; }
        public string DeviceBindingID { get; set; }
        public uint SessionProcessID { get; set; }

        public CommandFromWebEventArgs(int encoderNumber, WebCommandEnum webCommand, bool muted, int volume, bool isDeviceBinding, string deviceBindingID, uint sessionProcessID)
        {
            this.EncoderNumber = encoderNumber;
            this.WebCommand = webCommand;
            this.Muted = muted;
            this.Volume = volume;
            this.IsDeviceBinding = isDeviceBinding;
            this.DeviceBindingID = deviceBindingID;
            this.SessionProcessID = sessionProcessID;
        }
    }
}
