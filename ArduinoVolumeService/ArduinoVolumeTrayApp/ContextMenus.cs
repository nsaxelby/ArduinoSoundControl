using System;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
	class ContextMenus
	{
		bool isAboutLoaded = false;
		bool isSettingsLoaded = false;

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

			var statusLabelItem = new ToolStripLabel();
			statusLabelItem.Text = "Status";
			menu.Items.Add(statusLabelItem);

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
			if(!isSettingsLoaded)
            {
				isSettingsLoaded = true;
				new SettingsForm().ShowDialog();
				isSettingsLoaded = false;
            }
		}

		void About_Click(object sender, EventArgs e)
		{
			if (!isAboutLoaded)
			{
				isAboutLoaded = true;
				new AboutForm().ShowDialog();
				isAboutLoaded = false;
			}
		}

		void Exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
	}
}
