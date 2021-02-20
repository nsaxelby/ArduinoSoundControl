using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;

namespace ArduinoVolumeLib
{
    // Although this class is called 'SoundDevice' it is not akin to MMDevice. It's not strictly a device.
    // It's either a device ( as in master control of the device as a whole ), or it's an application 
    // So I can control a master volume of a device, or I can control the volume of an application
    public class SoundDevice : IAudioSessionEventsHandler
    {
        public bool IsMasterVolumeControlForDevice { get; set; }
        public MMDevice _device;
        private readonly AudioSessionControl _session;
        // Reference so we can call the vol changed event on parent
        private DeviceController _deviceController;

        public SoundDevice(DeviceController deviceController, bool isMasterVolumeForDevice, MMDevice device, AudioSessionControl session)
        {
            this.IsMasterVolumeControlForDevice = isMasterVolumeForDevice;
            this._device = device;
            this._session = session;
            _deviceController = deviceController;
            _session.RegisterEventClient(this);
        }

        public SoundDevice(DeviceController deviceController, bool isMasterVolumeForDevice, MMDevice device)
        {
            this.IsMasterVolumeControlForDevice = isMasterVolumeForDevice;
            this._device = device;
            _deviceController = deviceController;
            _device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            _deviceController.VolChangedDevice(_device.FriendlyName, data.MasterVolume, data.Muted, GetDeviceUniqueID());
        }
        public string GetDeviceUniqueID()
        {
            if(this.IsMasterVolumeControlForDevice)
            {
                return _device.ID;
            }
            else
            {
                return _session.GetSessionIdentifier;
            }
        }

        public string GetDeviceDisplayName()
        {
            if (this.IsMasterVolumeControlForDevice)
            {
                return _device.FriendlyName;
            }
            else
            {
                return _session.DisplayName;
            }
        }

        public bool GetDeviceMuted()
        {
            if (this.IsMasterVolumeControlForDevice)
            {
                return _device.AudioEndpointVolume.Mute;
            }
            else
            {
                return _session.SimpleAudioVolume.Mute;
            }
        }

        public float GetDeviceCurVol()
        {
            if (this.IsMasterVolumeControlForDevice)
            {
                return _device.AudioEndpointVolume.MasterVolumeLevelScalar;
            }
            else
            {
                return _session.SimpleAudioVolume.Volume;
            }
        }

        public void VolumeUp(float volAdjustAmountInc)
        {
            if(IsMasterVolumeControlForDevice)
            {
                float volume;
                volume = _device.AudioEndpointVolume.MasterVolumeLevelScalar + volAdjustAmountInc;
                if (volume >= 1F)
                {
                    volume = 1F;
                }
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            }
            else
            {
                float volume;
                volume = _session.SimpleAudioVolume.Volume + volAdjustAmountInc;
                if (volume >= 1F)
                {
                    volume = 1F;
                }
                _session.SimpleAudioVolume.Volume = volume;
            }
        }

        public void VolumeDown(float volAdjustmentDec)
        {
            if(IsMasterVolumeControlForDevice)
            {
                float volume = _device.AudioEndpointVolume.MasterVolumeLevelScalar - volAdjustmentDec;
                if (volume <= 0F)
                {
                    volume = 0F;
                }
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            }
            else
            {
                float volume = _session.SimpleAudioVolume.Volume - volAdjustmentDec;
                if (volume <= 0F)
                {
                    volume = 0F;
                }
                _session.SimpleAudioVolume.Volume = volume;
            }
        }

        public void VolumeSet(float volume)
        {
            if (volume <= 0F)
            {
                volume = 0F;
            }
            if (volume >= 1F)
            {
                volume = 1F;
            }
            if (IsMasterVolumeControlForDevice)
            {
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            }
            else
            {
                _session.SimpleAudioVolume.Volume = volume;
            }
        }

        public void Mute()
        {
            if (IsMasterVolumeControlForDevice)
            {
                _device.AudioEndpointVolume.Mute = true;
            }
            else
            {
                _session.SimpleAudioVolume.Mute = true;
            }
        }

        public void MuteToggle()
        {
            if (IsMasterVolumeControlForDevice)
            {
                _device.AudioEndpointVolume.Mute = !_device.AudioEndpointVolume.Mute;
            }
            else
            {
                _session.SimpleAudioVolume.Mute = !_session.SimpleAudioVolume.Mute;
            }
        }

        public void UnMute()
        {
            if (IsMasterVolumeControlForDevice)
            {
                _device.AudioEndpointVolume.Mute = false;
            }
            else
            {
                _session.SimpleAudioVolume.Mute = false;
            }
        }

        public void OnVolumeChanged(float volume, bool isMuted)
        {
            _deviceController.VolChangedDevice(_session.DisplayName, volume, isMuted, GetDeviceUniqueID());
        }

        public void OnDisplayNameChanged(string displayName)
        {
            _deviceController.VolChangedDevice(displayName, _session.SimpleAudioVolume.Volume, _session.SimpleAudioVolume.Mute, GetDeviceUniqueID());
        }

        public void OnIconPathChanged(string iconPath)
        {
            // DO nothing, intentional
        }

        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex)
        {
            _deviceController.VolChangedDevice(_session.DisplayName, _session.SimpleAudioVolume.Volume, _session.SimpleAudioVolume.Mute, GetDeviceUniqueID());
        }

        public void OnGroupingParamChanged(ref Guid groupingId)
        {
            // DO nothing, intentional
        }

        public void OnStateChanged(AudioSessionState state)
        {
            // DO nothing, intentional
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            // DO nothing, intentional
        }
    }
}
