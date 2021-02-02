using System;
using System.IO.Ports;
using System.Threading;
using NAudio.CoreAudioApi;

namespace ArduinoVolumeConsole
{
    public class Program
    {
        static bool _continue;
        static SerialPort _serialPort;
        static float _volAdjustAmountInc = 0.05f;
        static float _volAdjustAmountDec = 0.05f;
        static MMDevice device;

        static void Main(string[] args)
        {
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            _continue = true;

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

        private static void ProcessCommandForEnc1(string message)
        {
            if(message.StartsWith("UP"))
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
            else if(message.StartsWith("DOWN"))
            {
                float volume;
                volume = device.AudioEndpointVolume.MasterVolumeLevelScalar - _volAdjustAmountDec;
                if(volume <= 0F)
                {
                    volume = 0F;
                }
                device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            }
            else if(message.StartsWith("PRESS"))
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            }
        }
    }
}
