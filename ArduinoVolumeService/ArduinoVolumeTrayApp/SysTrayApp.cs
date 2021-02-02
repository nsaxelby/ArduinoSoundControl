using ArduinoVolumeTrayApp.Properties;
using System;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
    class SysTrayApp : IDisposable
    {
        readonly NotifyIcon _ni;

		public SysTrayApp()
		{
			_ni = new NotifyIcon();
		}

		public void Display()
		{
			_ni.Icon = Resources.iconfinder_audio_console_44847;
			_ni.Text = "Arduino Volume Service";
			_ni.Visible = true;

			// Attach a context menu.
			_ni.ContextMenuStrip = new ContextMenus().Create();
		}

		public void Dispose()
		{
			_ni.Dispose();
		}
	}
}
