using System;
using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
    public partial class LogListForm : Form
    {
        public LogListForm(string[] logs)
        {
            InitializeComponent();
            foreach(string s in logs)
            {
                this.textBox1.AppendText(s);
                this.textBox1.AppendText(Environment.NewLine);
            }
        }
    }
}
