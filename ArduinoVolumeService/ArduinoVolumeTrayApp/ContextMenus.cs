using ArduinoVolumeLib;
using System;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
	class ContextMenus
	{
		bool _isAboutLoaded = false;
		ToolStripLabel _serialStatusLabel;
		ToolStripMenuItem _webBrowseButton;
		ToolStripLabel _webStatusLabel;
		public event EventHandler ExitClicked;
		ContextMenuStrip _menu = new ContextMenuStrip();
		string _webHostUrl = "http://localhost";

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

			_serialStatusLabel = new ToolStripLabel();
			_serialStatusLabel.Text = "Serial Status";
			_menu.Items.Add(_serialStatusLabel);

			sep = new ToolStripSeparator();
			_menu.Items.Add(sep);

			_webStatusLabel = new ToolStripLabel();
			_webStatusLabel.Text = "Web Status";
			_menu.Items.Add(_webStatusLabel);

			_webBrowseButton = new ToolStripMenuItem();
			_webBrowseButton.Text = "browse to page n/a";
            _webBrowseButton.Click += _webBrowseButton_Click;
			_menu.Items.Add(_webBrowseButton);

			sep = new ToolStripSeparator();
			_menu.Items.Add(sep);

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new EventHandler(Exit_Click);
			_menu.Items.Add(item);

			return _menu;
		}

        private void _webBrowseButton_Click(object sender, EventArgs e)
        {
			System.Diagnostics.Process.Start(_webHostUrl);
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

		public void UpdateSerialStatus(SerialStateChangeEventArgs stateChangeEventArgs)
        {
			if (_menu.InvokeRequired)
			{
				_menu.Invoke(new Action(() =>
				{
					_serialStatusLabel.Text = "Serial: " + stateChangeEventArgs.State.ToString() + " " + stateChangeEventArgs.PortName;
				}));
			}
			else
			{
				_serialStatusLabel.Text = "Serial: " + stateChangeEventArgs.State.ToString() + " " + stateChangeEventArgs.PortName;
			}
			
        }

		public void UpdateWebConnectorStatus(WebConnectorStateChangeEventArgs webStateArgs)
        {
			if (_menu.InvokeRequired)
			{
				_menu.Invoke(new Action(() =>
				{
					_webStatusLabel.Text = "Web: " + webStateArgs.State.ToString();
					_webHostUrl = FixUrlPlusSign(webStateArgs.Url);
					_webBrowseButton.Text = FixUrlPlusSign(webStateArgs.Url);
				}));
			}
			else
			{
				_webStatusLabel.Text = "Web: " + webStateArgs.State.ToString();
				_webHostUrl = FixUrlPlusSign(webStateArgs.Url);
				_webBrowseButton.Text = FixUrlPlusSign(webStateArgs.Url);
			}
		}

		public string FixUrlPlusSign(string inputURL)
        {
			return inputURL.Replace("+:", "localhost:");
        }
	}
}
