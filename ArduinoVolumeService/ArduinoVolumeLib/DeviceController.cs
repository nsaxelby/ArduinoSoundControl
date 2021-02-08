using NAudio.CoreAudioApi;
using System;

namespace ArduinoVolumeLib
{
    public class DeviceController
    {
        // I only have 3 rotary encoders
        SoundDevice[] _devices = new SoundDevice[3];
        float _volAdjustAmountInc = 0.05F;
        float _volAdjustAmountDec = 0.05F;

        public event EventHandler<DeviceVolChangedEventArgs> DeviceVolChangedEvent;

        public DeviceController()
        {
            // Setup audio devices
            var deviceEnumerator = new MMDeviceEnumerator();

            // Always initiate Encoder 1 to Master Volume of the default audio device
            if (deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia) != null)
            {
                _devices[0] = new SoundDevice(this, true, deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia));
            }
            // Try initiate always to Microphone if we find one
            if (deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia) != null)
            {
                _devices[1] = new SoundDevice(this, true, deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia));
            }
            // Find Chrome as default..
            var sessionChrome = FindChrome(deviceEnumerator);
            if(sessionChrome != null)
            {
                _devices[2] = new SoundDevice(this, false, deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia),sessionChrome);
            }
        }

        private AudioSessionControl FindChrome(MMDeviceEnumerator deviceEnumerator)
        {
            var coreDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            for (int i = 0; i < coreDevice.AudioSessionManager.Sessions.Count; i++)
            {
                var sess = coreDevice.AudioSessionManager.Sessions[i];
                if(sess.IsSystemSoundsSession == false)
                {
                    if(sess.GetSessionIdentifier.Contains("chrome"))
                    {
                        return sess;
                    }
                }
            }
            return null;
        }

        public void VolumeUp(int deviceNum)
        {
           if(_devices[deviceNum -1] != null)
            {
                _devices[deviceNum - 1].VolumeUp(_volAdjustAmountInc);
            }
        }

        internal void VolChangedDevice(string deviceFriendlyName, float masterVolume, bool muted)
        {
            RaiseDeviceVolChangedEvent(new DeviceVolChangedEventArgs(deviceFriendlyName, masterVolume, muted));
        }

        public void VolumeDown(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].VolumeDown(_volAdjustAmountDec);
            }
        }

        public void Mute(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].Mute();
            }
        }

        protected virtual void RaiseDeviceVolChangedEvent(DeviceVolChangedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<DeviceVolChangedEventArgs> raiseEvent = DeviceVolChangedEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }
    }
}
