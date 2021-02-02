using System;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
	class ContextMenus
	{
		bool _isAboutLoaded = false;
		bool _isSettingsLoaded = false;
		ToolStripLabel _statusLabel;

		public ContextMenuStrip Create()
		{
			// Add the default menu options.
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			item = new ToolStripMenuItem();
			item.Text = "Settings";
			item.Click += new EventHandler(Settings_Click);
			menu.Items.Add(item);

			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			menu.Items.Add(item);

			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			_statusLabel = new ToolStripLabel();
			_statusLabel.Text = "Status";
			menu.Items.Add(_statusLabel);

			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new EventHandler(Exit_Click);
			menu.Items.Add(item);

			return menu;
		}

		void Settings_Click(object sender, EventArgs e)
		{
			if(!_isSettingsLoaded)
            {
				_isSettingsLoaded = true;
				new SettingsForm().ShowDialog();
				_isSettingsLoaded = false;
            }
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
			Application.Exit();
		}
	}
}
