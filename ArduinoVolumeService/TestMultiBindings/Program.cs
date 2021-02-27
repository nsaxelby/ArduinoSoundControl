using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;

namespace TestMultiBindings
{
    class Program
    {
        static void Main(string[] args)
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            SoundDevice dev = null;

            Console.WriteLine("Started APP  - type 'end' to end or press enter to find chrome, and bind to it");
            while (Console.ReadLine() != "end")
            {
                Console.WriteLine("Finding chrome");
                var chrome = FindChrome(deviceEnumerator);
                if (chrome != null)
                {
                    Console.WriteLine("Chrome found, binding to device");
                    if(dev != null)
                    {
                        dev.Dispose();
                    }
                    dev = new SoundDevice(chrome);
                }
                else
                {
                    Console.WriteLine("Chrome not found");
                }
                Console.WriteLine("-------");
                Console.WriteLine("type 'end' to end or press enter to find chrome, and bind to it");
            }
        }

        private static AudioSessionControl FindChrome(MMDeviceEnumerator deviceEnumerator)
        {
            var coreDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            for (int i = 0; i < coreDevice.AudioSessionManager.Sessions.Count; i++)
            {
                var sess = coreDevice.AudioSessionManager.Sessions[i];
                if (sess.IsSystemSoundsSession == false)
                {
                    if (sess.GetSessionIdentifier.Contains("chrome"))
                    {
                        return sess;
                    }
                }
            }
            return null;
        }
    }

    public class SoundDevice : IAudioSessionEventsHandler
    {

        private AudioSessionControl _session;

        public SoundDevice(AudioSessionControl sess)
        {
            _session = sess;
            _session.RegisterEventClient(this);
        }
        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex)
        {
            throw new NotImplementedException();
        }

        public void OnDisplayNameChanged(string displayName)
        {
            throw new NotImplementedException();
        }

        public void OnGroupingParamChanged(ref Guid groupingId)
        {
            throw new NotImplementedException();
        }

        public void OnIconPathChanged(string iconPath)
        {
            throw new NotImplementedException();
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            throw new NotImplementedException();
        }

        public void OnStateChanged(AudioSessionState state)
        {
            throw new NotImplementedException();
        }

        public void OnVolumeChanged(float volume, bool isMuted)
        {
            Console.WriteLine("Vol changed to :" + volume.ToString());
        }

        public void Dispose()
        {
            if(_session != null)
            {
                _session.UnRegisterEventClient(this);
                // I think Dispose calls UnRegisterEventClient anyway.. But belt and braces
                _session.Dispose();
            }
        }
    }
}