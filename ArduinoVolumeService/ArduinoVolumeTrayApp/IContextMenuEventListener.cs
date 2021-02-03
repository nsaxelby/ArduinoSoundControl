using System;

namespace ArduinoVolumeTrayApp
{
    public interface IContextMenuEventListener
    {
        event EventHandler ExitClicked;
    }
}
