using System;

namespace ArduinoVolumeLib
{
    public class WebConnectorStateChangeEventArgs : EventArgs
    {
        public WebStateEnum State { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public WebConnectorStateChangeEventArgs(string message, WebStateEnum state, string url)
        {
            this.State = state;
            this.Message = message;
            this.Url = url;
        }
    }
}
