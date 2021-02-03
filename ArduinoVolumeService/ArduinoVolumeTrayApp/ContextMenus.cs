using ArduinoVolumeLib;
using System;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
	class ContextMenus
	{
		bool _isAboutLoaded = false;
		ToolStripLabel _statusLabel;
		public event EventHandler ExitClicked;
		ContextMenuStrip _menu = new ContextMenuStrip();

		public ContextMenuStrip Create()
		{
			// Add the default menu options.
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			_menu.Items.Add(item);

			sep = new ToolStripSeparator();
			_menu.Items.Add(sep);

			_statusLabel = new ToolStripLabel();
			_statusLabel.Text = "Status";
			_menu.Items.Add(_statusLabel);

			sep = new ToolStripSeparator();
			_menu.Items.Add(sep);

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new EventHandler(Exit_Click);
			_menu.Items.Add(item);

			return _menu;
		}

		void About_Click(object sender, EventArgs e)
		{
			if (!_isAboutLoaded)
			{
				_isAboutLoaded = true;
				new AboutForm().ShowDialog();
				_isAboutLoaded = false;
			}
		}

		void Exit_Click(object sender, EventArgs e)
		{
			if (ExitClicked != null)
			{
				ExitClicked.Invoke(this, e);
			}
			else
            {
				Application.Exit();
            }
		}

		public void UpdateStatus(SerialStateChangeEventArgs stateChangeEventArgs)
        {
			if (_menu.InvokeRequired)
			{
				_menu.Invoke(new Action(() =>
				{
					_statusLabel.Text = stateChangeEventArgs.State.ToString() + " " + stateChangeEventArgs.PortName;
				}));
			}
			else
			{
				_statusLabel.Text = stateChangeEventArgs.State.ToString() + " " + stateChangeEventArgs.PortName;
			}
			
        }
	}
}
