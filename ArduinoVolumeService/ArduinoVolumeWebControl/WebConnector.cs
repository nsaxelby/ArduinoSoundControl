﻿using ArduinoVolumeLib;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Threading;
using System.Web.Http;
using Microsoft.Owin.Cors;


namespace ArduinoVolumeWebControl
{
    public interface IWebConnector
    {
        void VolumeChangeFromWeb(int encoder, int volume);
        void EncoderMutedChangeFromWeb(int encoder, bool muted);
        void RebindController(int encoder, bool deviceBinding, string deviceID, uint sessionProcessID);
        event EventHandler<DeviceVolChangedEventArgs> DeviceVolChangedEvent;
        event EventHandler<CommandFromWebEventArgs> WebCommandEvent;
        event EventHandler<WebConnectorStateChangeEventArgs> WebStateChangeEvent;
        event EventHandler<RequestBoundDevicesEventArgs> WebRequestBoundDevicesChangeEvent;

        void RequestBoundDevices();

        void UpdateDevicesAndBindings(DeviceListResponseEventArgs e);
    }

    public class WebConnector : IWebConnector
    {
        public bool _continue = true;
        public Thread _runThread;
        public string _webUrlString;

        public event EventHandler<DeviceVolChangedEventArgs> DeviceVolChangedEvent;
        public event EventHandler<CommandFromWebEventArgs> WebCommandEvent;
        public event EventHandler<WebConnectorStateChangeEventArgs> WebStateChangeEvent;
        public event EventHandler<RequestBoundDevicesEventArgs> WebRequestBoundDevicesChangeEvent;

        public int msDelayDontUpdateWeb = 2000;
        public DateTime dateLastUpdateFromWeb = new DateTime();

        public WebConnector(string webUrl)
        {
            _webUrlString = webUrl;
            _runThread = new Thread(RunWeb);
            RaiseStateChangeEvent(new WebConnectorStateChangeEventArgs("Connector Made", WebStateEnum.Stopped, _webUrlString));
        }

        public void StartWeb()
        {
            _runThread.Start();
        }

        private void RunWeb()
        {
            RaiseStateChangeEvent(new WebConnectorStateChangeEventArgs("Starting", WebStateEnum.Starting, _webUrlString));

            try
            {
                Console.WriteLine("Starting web server");
                using (WebApp.Start(_webUrlString, (app) =>
                 {
                     GlobalHost.DependencyResolver.Register(typeof(WebControlHub), () => new WebControlHub(this));

                     HttpConfiguration config = new HttpConfiguration();
                     config.Routes.MapHttpRoute(
                         "API Default", "api/{controller}/{id}",
                         new { id = RouteParameter.Optional });

                     config.Routes.MapHttpRoute(
                         "Static", "{*url}",
                         new { controller = "StaticFiles", action = "Index" });

                     app.UseCors(CorsOptions.AllowAll);
                     app.MapSignalR("/signalr", new HubConfiguration());
                     app.UseWebApi(config);
                 }))
                {
                    Console.WriteLine("Started web server on " + _webUrlString);
                    RaiseStateChangeEvent(new WebConnectorStateChangeEventArgs("Started", WebStateEnum.Running, _webUrlString));
                    while (_continue)
                    {
                        Thread.Sleep(500);
                        if (_runThread.ThreadState != ThreadState.Running)
                        {
                            break;
                        }
                    }
                };
                Console.WriteLine("Ended web server");
                RaiseStateChangeEvent(new WebConnectorStateChangeEventArgs("Stopped", WebStateEnum.Stopped, _webUrlString));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Web server exited with exception: " + ex.Message);
            }
            RaiseStateChangeEvent(new WebConnectorStateChangeEventArgs("Error", WebStateEnum.Error, _webUrlString));
        }

        protected virtual void RaiseStateChangeEvent(WebConnectorStateChangeEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<WebConnectorStateChangeEventArgs> raiseEvent = WebStateChangeEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        protected virtual void DeviceListChangedEvent(DeviceListResponseEventArgs e)
        {
            GlobalHost.ConnectionManager.GetHubContext<WebControlHub>().Clients.All.updateVol(e);
        }

        public void VolumeChangeFromWeb(int encoder, int volume)
        {
            dateLastUpdateFromWeb = DateTime.Now;
            RaiseWebCommandEvent(new CommandFromWebEventArgs(encoder, WebCommandEnum.VOLCHANGE, false, volume, false, "", 0));
        }

        public void EncoderMutedChangeFromWeb(int encoder, bool muted)
        {
            RaiseWebCommandEvent(new CommandFromWebEventArgs(encoder, muted ? WebCommandEnum.MUTED : WebCommandEnum.UNMUTED, muted, 0, false, "", 0));
        }

        public void RebindController(int encoder, bool deviceBinding, string deviceID, uint sessionProcessID)
        {
            RaiseWebCommandEvent(new CommandFromWebEventArgs(encoder, WebCommandEnum.REBINDENCODER, false, 0, deviceBinding, deviceID, sessionProcessID));
        }

        protected virtual void RaiseWebCommandEvent(CommandFromWebEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<CommandFromWebEventArgs> raiseEvent = WebCommandEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        public void SendVolChangedEncoderNumber(DeviceVolChangedEventArgs e)
        {
            if(e.EncoderNumbers.Count >= 1)
            {
                RaiseDeviceVolChangedEvent(e);
            }
        }
        public void SendMutedChangeEnocderNumber(DeviceVolChangedEventArgs e)
        {
            if(e.EncoderNumbers.Count >= 1)
            {
                RaiseDeviceVolChangedEvent(e);
            }
        }

        protected virtual void RaiseDeviceVolChangedEvent(DeviceVolChangedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<DeviceVolChangedEventArgs> raiseEvent = DeviceVolChangedEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }

            GlobalHost.ConnectionManager.GetHubContext<WebControlHub>().Clients.All.updateVol(e.EncoderNumbers, e.Muted, e.Volume * 100);
        }

        public void StopWeb()
        {
            _continue = false;
        }

        public void RequestBoundDevices()
        {
            RequestBoundDevicesEvent();
        }

        private void RequestBoundDevicesEvent()
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<RequestBoundDevicesEventArgs> raiseEvent = WebRequestBoundDevicesChangeEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, new RequestBoundDevicesEventArgs());
            }
        }

        public void UpdateDevicesAndBindings(DeviceListResponseEventArgs e)
        {
            GlobalHost.ConnectionManager.GetHubContext<WebControlHub>().Clients.All.updateDevices(e);
        }
    }

    public class WebControlHub : Hub
    {
        private IWebConnector _webConnector;
        private Guid _hubGuid = Guid.NewGuid();
        public WebControlHub(IWebConnector webCon)
        {
            _webConnector = webCon;
            Console.WriteLine("Created new hub" + _hubGuid);
        }

        public void ChangeVol(int enocder, int volume)
        {
            _webConnector.VolumeChangeFromWeb(enocder, volume);
        }

        public void RequestBoundDevices()
        {
            _webConnector.RequestBoundDevices();
        }

        public void MuteEncoder(int encoder, bool mute)
        {
            _webConnector.EncoderMutedChangeFromWeb(encoder, mute);
        }

        public void RebindEncoderToDevice(int encoder, string deviceID)
        {
            _webConnector.RebindController(encoder, true, deviceID, 0);
        }

        public void RebindEncoderToSession(int encoder, string deviceID, int sessionProcessID)
        {
            _webConnector.RebindController(encoder, false, deviceID, Convert.ToUInt32(sessionProcessID));
        }
    }
}
