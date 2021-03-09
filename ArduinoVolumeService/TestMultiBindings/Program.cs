using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestMultiBindings
{
    class Program
    {
        static void Main(string[] args)
        {
            MainRunner mr = new MainRunner();
            mr.Run();
        }
    }

    public class MainRunner : IMMNotificationClient
    {
        public Dictionary<string, MMDevice> deviceList = new Dictionary<string, MMDevice>();
        MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        Dictionary<string, SoundSession> soundSessions = new Dictionary<string, SoundSession>();

        public void Run()
        {
            AddAllDevicesAndSessions();

            deviceEnumerator.RegisterEndpointNotificationCallback(this);

            Console.WriteLine("Started APP  - type 'end' to end or press enter to find chrome, and bind to it");
            while (Console.ReadLine() != "end")
            {
                Console.WriteLine("Finding chrome");
                bool resultfind = FindChrome();

                if (resultfind)
                {
                    Console.WriteLine("Chrome found");
                }
                else
                {
                    Console.WriteLine("Chrome not found");
                }
                Console.WriteLine("-------");
                Console.WriteLine("type 'end' to end or press enter to find chrome, and bind to it");
            }
        }

        private void AddAllDevicesAndSessions()
        {
            foreach(var devices in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
            {
                AddDeviceObj(devices);
            }
        }

        private void AddDeviceObj(MMDevice device)
        {
            if (deviceList.ContainsKey(device.ID) == false)
            {
                var asm = device.AudioSessionManager;
                for (int i = 0; i < asm.Sessions.Count; i++)
                {
                    var session = asm.Sessions[i];
                    if (session.IsSystemSoundsSession == false)
                    {
                        AddSession(session);
                    }
                }

                asm.OnSessionCreated += AudioSessionManager_OnSessionCreated;
                deviceList.Add(device.ID, device);
            }
        }

        private void AddSession(AudioSessionControl sessCont)
        {
            if(soundSessions.ContainsKey(sessCont.GetSessionIdentifier) == false)
            {
                Console.WriteLine("added session " + sessCont.GetSessionIdentifier + " dname: " + sessCont.DisplayName + " instanceid : " + sessCont.GetSessionInstanceIdentifier);
                SoundSession ss = new SoundSession(sessCont, this);
                soundSessions.Add(sessCont.GetSessionIdentifier, ss);
            }
        }

        private void AudioSessionManager_OnSessionCreated(object sender, IAudioSessionControl newSession)
        {
            AudioSessionControl asc = new AudioSessionControl(newSession);
            Console.WriteLine("New AudioSessionControl from OnSessionCreated");
            AddSession(asc);
        }

        public void RemoveExpiredSession(string identifier)
        {
            if (soundSessions.ContainsKey(identifier) == true)
            {
                Console.WriteLine("removed session expired " + identifier);
                soundSessions[identifier].Dispose();
                soundSessions.Remove(identifier);
            }
        }

        private void AddDeviceID(string id)
        {
            if (deviceList.ContainsKey(id) == false)
            {
                Console.WriteLine("added device " + id);
                AddDeviceObj(deviceEnumerator.GetDevice(id));
            }
        }

        private void RemoveDeviceID(string id)
        {
            if (deviceList.ContainsKey(id) == true)
            {
                Console.WriteLine("Removed device " + id);
                var devToRemove = deviceList[id];

                devToRemove.AudioSessionManager.Dispose();
                devToRemove.Dispose();
                
                deviceList.Remove(id);
                devToRemove = null;
                GC.Collect();
            }
        }

        private bool FindChrome()
        {
            foreach(var sess in soundSessions)
            {
                if (sess.Value.GetIdentifier().Contains("chrome"))
                {
                    Console.WriteLine("Found chrome in other sessions");
                    return true;
                }
            }
            return false;
        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            Console.WriteLine("Def dev changed");
            AddDeviceID(defaultDeviceId);
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            Console.WriteLine("Device added");
            AddDeviceID(pwstrDeviceId);
        }

        public void OnDeviceRemoved(string deviceId)
        {
            Console.WriteLine("Device demoved");
            RemoveDeviceID(deviceId);
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            if (newState != DeviceState.Active)
            {
                RemoveDeviceID(deviceId);
            }
            else if(newState == DeviceState.Active)
            {
                AddDeviceID(deviceId);
            }
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {

        }
    }

    public class SoundSession : IAudioSessionEventsHandler
    {

        private AudioSessionControl _session;
        private MainRunner _parent;
        private string _sessionOriginalID;

        public SoundSession(AudioSessionControl sess, MainRunner parent)
        {
            _session = sess;
            _sessionOriginalID = sess.GetSessionIdentifier;
            _parent = parent;
            _session.RegisterEventClient(this);
        }

        public string GetIdentifier()
        {
            return _session.GetSessionIdentifier;
        }
        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex)
        {
        }

        public void OnDisplayNameChanged(string displayName)
        {
        }

        public void OnGroupingParamChanged(ref Guid groupingId)
        {
        }

        public void OnIconPathChanged(string iconPath)
        {
        }

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            if (_session != null)
            {
                Console.WriteLine("Session disconnected removing");
                _session.UnRegisterEventClient(this);
                _parent.RemoveExpiredSession(_sessionOriginalID);
                _session = null;
            }
        }

        public void OnStateChanged(AudioSessionState state)
        {
            if (state == AudioSessionState.AudioSessionStateExpired)
            {
                if (_session != null)
                {
                    Console.WriteLine("Sess expired session removing");

                    _session.UnRegisterEventClient(this);
                    _parent.RemoveExpiredSession(_sessionOriginalID);
                }
            }
        }

        public void OnVolumeChanged(float volume, bool isMuted)
        {
            Console.WriteLine("Vol changed to :" + volume.ToString() + "For Session : " + _session.GetSessionIdentifier);
        }

        public void Dispose()
        {
            _session.Dispose();
            _session = null;
        }
    }
}