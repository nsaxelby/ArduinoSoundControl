using ArduinoVolumeLib;
using ArduinoVolumeTrayApp.Properties;
using ArduinoVolumeWebControl;
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
        DeviceController _deviceController;
        WebConnector _webAccessHost;

        private static string _webHostUrl = "http://localhost:5150";

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

            // Setup serial connector
            _serialCon = new SerialConnector();
            _serialCon.StateChangeEvent += _serialCon_StateChangeEvent;
            _serialCon.CommandReceivedEvent += _serialCon_CommandReceivedEvent;
			_serialCon.Connect();

            _deviceController = new DeviceController();
            _deviceController.DeviceVolChangedEvent += _deviceController_DeviceVolChangedEvent;

            _connKeepAliveThread = new Thread(KeepConnAlive);
            _connKeepAliveThread.Start();

            _webAccessHost = new WebConnector(_webHostUrl);
            _webAccessHost.WebStateChangeEvent += _webAccessHost_WebStateChangeEvent;
            _webAccessHost.StartWeb();
        }

        private void _webAccessHost_WebStateChangeEvent(object sender, WebConnectorStateChangeEventArgs e)
        {
            _contextMenus.UpdateWebConnectorStatus(e);
        }

        private void _deviceController_DeviceVolChangedEvent(object sender, DeviceVolChangedEventArgs e)
        {
            if(e.Muted)
            {
                _serialCon.SendMutedChange(e.Name);
            }
            else
            {
                _serialCon.SendMVolChange(e.Name, e.Volume);
            }
        }

        private void _serialCon_CommandReceivedEvent(object sender, CommandFromArduinoEventArgs e)
        {
            switch (e.Command)
            {
                case CommandsEnum.UP:
                    {
                        _deviceController.VolumeUp(e.EncoderNumber);
                    }
                    break;
                case CommandsEnum.DOWN:
                    {
                        _deviceController.VolumeDown(e.EncoderNumber);
                    }
                    break;
                case CommandsEnum.PRESS:
                    {
                        _deviceController.Mute(e.EncoderNumber);
                    }
                    break;
                default:
                    break;
            }
        }

        private void _serialCon_StateChangeEvent(object sender, SerialStateChangeEventArgs e)
        {
             _contextMenus.UpdateSerialStatus(e);
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

            try
            {
                _webAccessHost.StopWeb();
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
