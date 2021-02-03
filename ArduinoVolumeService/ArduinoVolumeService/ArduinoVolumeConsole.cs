using ArduinoVolumeLib;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArduinoVolumeConsole
{
    public class ArduinoVolumeConsole
    {
        static bool _continue;
        static SerialPort _serialPort;
        static float _volAdjustAmountInc = 0.05f;
        static float _volAdjustAmountDec = 0.05f;
        static MMDevice device;
        void Main(string[] args)
        {
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(Read);

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = "COM3";

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.NewLine = ";";

            _serialPort.Open();
            _continue = true;
            readThread.Start();

            var deviceEnumerator = new MMDeviceEnumerator();
            device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            SendMVolChange("Master Volume", device.AudioEndpointVolume.MasterVolumeLevelScalar);
            device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            Console.WriteLine("Running service, type quit to exit.");

            while (_continue)
            {
                message = Console.ReadLine();

                if (stringComparer.Equals("quit", message))
                {
                    _continue = false;
                }
                else
                {
                    _serialPort.WriteLine(
                        String.Format(": {0}", message));
                }
            }

            readThread.Join();
            _serialPort.Close();
        }

        private static void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (data.Muted == true)
            {
                SendMutedChange("Master Volume   ");
            }
            else
            {
                SendMVolChange("Master Volume   ", data.MasterVolume);
            }
        }

        private static void SendMVolChange(String medType, float inputVol)
        {
            _serialPort.Write("ROW1:" + medType + ";");
            string row2line = "ROW2:" + Util.VolumeToRow2Bars(inputVol * 100) + ";";
            Console.WriteLine(row2line);
            _serialPort.Write(row2line);
        }

        private static void SendMutedChange(String medType)
        {
            _serialPort.Write("ROW1:" + medType + ";");
            string row2line = "ROW2:" + "-----MUTED------;";
            Console.WriteLine(row2line);
            _serialPort.Write(row2line);
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    ProcessMessage(message);
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }

        private static void ProcessMessage(string message)
        {
            if (message.StartsWith("UP-1") || message.StartsWith("DOWN-1") || message.StartsWith("PRESS-1"))
            {
                ProcessCommandForEnc1(message);
            }
            else if (message.StartsWith("UP-2") || message.StartsWith("DOWN-2") || message.StartsWith("PRESS-2"))
            {
                ProcessCommandForEnc2(message);
            }
            else if (message.StartsWith("UP-3") || message.StartsWith("DOWN-3") || message.StartsWith("PRESS-3"))
            {
                ProcessCommandForEnc3(message);
            }
        }

        private static void ProcessCommandForEnc3(string message)
        {

        }

        private static void ProcessCommandForEnc2(string message)
        {

        }

        private static void ProcessCommandForEnc1(string message)
        {
            if (message.StartsWith("UP"))
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
            else if (message.StartsWith("DOWN"))
            {
                float volume;
                volume = device.AudioEndpointVolume.MasterVolumeLevelScalar - _volAdjustAmountDec;
                if (volume <= 0F)
                {
                    volume = 0F;
                }
                device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            }
            else if (message.StartsWith("PRESS"))
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            }
        }
    }
}
