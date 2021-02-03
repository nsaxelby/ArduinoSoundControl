using System.Windows.Forms;

namespace ArduinoVolumeTrayApp
{
    class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show the system tray icon.
            using (SysTrayApp pi = new SysTrayApp())
            {
                // Make sure the application runs!
                Application.Run();
            }
        }
    }
}
