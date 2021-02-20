using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;

namespace ArduinoVolumeLib
{
    public class DeviceController
    {
        // I only have 3 rotary encoders
        SoundDevice[] _devices = new SoundDevice[3];
        float _volAdjustAmountInc = 0.05F;
        float _volAdjustAmountDec = 0.05F;
        MMDeviceEnumerator deviceEnumerator;

        public event EventHandler<DeviceVolChangedEventArgs> DeviceVolChangedEvent;
        public event EventHandler<DeviceListChangedEventArgs> DeviceListChangedEvent;

        public DeviceController()
        {
            // Setup audio devices
            deviceEnumerator = new MMDeviceEnumerator();

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
            if (sessionChrome != null)
            {
                _devices[2] = new SoundDevice(this, false, deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia), sessionChrome);
            }
            else
            {
                var sessionSpotify = FindSpotify(deviceEnumerator);
                if (sessionSpotify != null)
                {
                    _devices[2] = new SoundDevice(this, false, deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia), sessionSpotify);

                }
            }
        }

        private AudioSessionControl FindSpotify(MMDeviceEnumerator deviceEnumerator)
        {
            var coreDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            for (int i = 0; i < coreDevice.AudioSessionManager.Sessions.Count; i++)
            {
                var sess = coreDevice.AudioSessionManager.Sessions[i];
                if (sess.IsSystemSoundsSession == false)
                {
                    if (sess.GetSessionIdentifier.Contains("Spotify"))
                    {
                        return sess;
                    }
                }
            }
            return null;
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

        public void VolumeSet(int deviceNum, int volume)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].VolumeSet((float)(Convert.ToDecimal(volume) / (decimal)100));
            }
        }

        internal void VolChangedDevice(string deviceFriendlyName, float masterVolume, bool muted, string identifier)
        {
            int encoderNumber = GetEncoderNumFromID(identifier);
            RaiseDeviceVolChangedEvent(new DeviceVolChangedEventArgs(deviceFriendlyName, masterVolume, muted, encoderNumber));
        }

        private int GetEncoderNumFromID(string identifier)
        {
            int num = 1;
            foreach(var device in _devices)
            {
                if(device != null)
                {
                    if (device.GetDeviceUniqueID() == identifier)
                    {
                        return num;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            return -1;
        }

        public void VolumeDown(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].VolumeDown(_volAdjustAmountDec);
            }
        }

        public void GetDeviceStatus(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                var soundDev = _devices[deviceNum - 1];
                VolChangedDevice(soundDev.GetDeviceDisplayName(),
                    soundDev.GetDeviceCurVol(),
                    soundDev.GetDeviceMuted(),
                    soundDev.GetDeviceUniqueID());
            }
        }

        public void Mute(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].Mute();
            }
        }

        public void MuteToggle(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].MuteToggle();
            }
        }

        public void UnMute(int deviceNum)
        {
            if (_devices[deviceNum - 1] != null)
            {
                _devices[deviceNum - 1].UnMute();
            }
        }

        public void RequestDevices()
        {
            List<DeviceItem> devices = new List<DeviceItem>();
            var allDevices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach(var dev in allDevices)
            {
                DeviceItem di = new DeviceItem(dev.DeviceFriendlyName, dev.ID, GetDeviceEncoderNumberByID(dev.ID));
                for(int i = 0; i < dev.AudioSessionManager.Sessions.Count; i++)
                {
                    var sess = dev.AudioSessionManager.Sessions[i];
                    SoundSessionItem sessItem = new SoundSessionItem(sess.DisplayName, sess.GetSessionIdentifier, GetDeviceEncoderNumberByID(sess.GetSessionIdentifier));
                    di.SoundSessions.Add(sessItem);
                }
                devices.Add(di);
            }
            RaiseDeviceListChangedEvent(new DeviceListChangedEventArgs(devices));
        }

        public int? GetDeviceEncoderNumberByID(string deviceID)
        {
            for(int i = 0; i <_devices.Length; i++)
            {
                if(_devices[i] != null)
                {
                    if(_devices[i].GetDeviceUniqueID() == deviceID)
                    {
                        return i + 1;
                    }
                }
            }
            return null;
        }

        protected virtual void RaiseDeviceListChangedEvent(DeviceListChangedEventArgs e)
        {
            EventHandler<DeviceListChangedEventArgs> raiseEvent = DeviceListChangedEvent;

            if(raiseEvent != null)
            {
                raiseEvent(this, e);
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
