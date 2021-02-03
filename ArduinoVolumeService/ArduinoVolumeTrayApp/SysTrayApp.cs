using ArduinoVolumeLib;
using ArduinoVolumeTrayApp.Properties;
using NAudio.CoreAudioApi;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
    class SysTrayApp : IDisposable
    {
        readonly NotifyIcon _ni;
		SerialConnector _serialCon;
		ContextMenus _contextMenus;
        MMDevice[] _devices = new MMDevice[3];
        float _volAdjustAmountInc = 0.05F;
        float _volAdjustAmountDec = 0.05F;
        Thread _connKeepAliveThread;
        bool _continue = true;
        bool _keepConnected = true;


        public SysTrayApp()
		{
			// Setup tray icon
			_ni = new NotifyIcon();
			_ni.Icon = Resources.iconfinder_audio_console_44847;
			_ni.Text = "Arduino Volume Service";
			_ni.Visible = true;

			// Setup tray icon to have a menu
			_contextMenus = new ContextMenus();
			_contextMenus.ExitClicked += _contextMenus_ExitClicked;
			_ni.ContextMenuStrip = _contextMenus.Create();

            // Setup audio devices
            var deviceEnumerator = new MMDeviceEnumerator();
            _devices[0] = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _devices[1] = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _devices[2] = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            //SendMVolChange("Master Volume", device.AudioEndpointVolume.MasterVolumeLevelScalar);
            _devices[0].AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            // Setup serial connector
            _serialCon = new SerialConnector();
            _serialCon.StateChangeEvent += _serialCon_StateChangeEvent;
            _serialCon.CommandReceivedEvent += _serialCon_CommandReceivedEvent;
			_serialCon.Connect();

            _serialCon.SendMVolChange(_devices[0].DeviceFriendlyName, _devices[0].AudioEndpointVolume.MasterVolumeLevelScalar);

            _connKeepAliveThread = new Thread(KeepConnAlive);
            _connKeepAliveThread.Start();
		}

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (data.Muted == true)
            {
                _serialCon.SendMutedChange("Master Volume");
            }
            else
            {
                _serialCon.SendMVolChange("Master Volume", data.MasterVolume);
            }
        }

        private void _serialCon_CommandReceivedEvent(object sender, CommandFromArduinoEventArgs e)
        {
            var device = _devices[e.EncoderNumber - 1];
            switch (e.Command)
            {
                case CommandsEnum.UP:
                    {
                        if (device.AudioEndpointVolume.Mute == true)
                        {
                            // don't allow turning volume UP when muted
                        }
                        else
                        {
                            float volume;
                            volume = device.AudioEndpointVolume.MasterVolumeLevelScalar + _volAdjustAmountInc;
                            if (volume >= 1F)
                            {
                                volume = 1F;
                            }
                            device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
                        }
                    }
                    break;
                case CommandsEnum.DOWN:
                    {
                        float volume = device.AudioEndpointVolume.MasterVolumeLevelScalar - _volAdjustAmountDec;
                        if (volume <= 0F)
                        {
                            volume = 0F;
                        }
                        device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
                    }
                    break;
                case CommandsEnum.PRESS:
                    {
                        device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    }
                    break;
                default:
                    break;
            }
        }

        private void _serialCon_StateChangeEvent(object sender, SerialStateChangeEventArgs e)
        {
             _contextMenus.UpdateStatus(e);
        }

        private void _contextMenus_ExitClicked(object sender, EventArgs e)
        {
            try
            {
                _continue = false;
                _serialCon.Disconnect();
            }
			catch
            {

            }
			Application.Exit();
        }

        public void Dispose()
		{
			_ni.Dispose();
		}

        public void KeepConnAlive()
        {
            while (_continue)
            {
                try
                {
                    if(_keepConnected)
                    {
                        if(_serialCon.IsConnected() == false)
                        {
                            _serialCon.Connect();
                        }
                    }
                }
                catch (TimeoutException) { }
                Thread.Sleep(5000);
            }
        }
    }
}
