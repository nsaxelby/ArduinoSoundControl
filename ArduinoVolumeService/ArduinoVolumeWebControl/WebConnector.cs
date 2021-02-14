using ArduinoVolumeLib;
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
        void RebindController(int encoder, bool deviceBinding, string deviceID, string sessionID);
        event EventHandler<DeviceVolChangedEventArgs> DeviceVolChangedEvent;
        event EventHandler<CommandFromWebEventArgs> WebCommandEvent;
        event EventHandler<WebConnectorStateChangeEventArgs> WebStateChangeEvent;
    }

    public class WebConnector : IWebConnector
    {
        public bool _continue = true;
        public Thread _runThread;
        public string _webUrlString;

        public event EventHandler<DeviceVolChangedEventArgs> DeviceVolChangedEvent;
        public event EventHandler<CommandFromWebEventArgs> WebCommandEvent;
        public event EventHandler<WebConnectorStateChangeEventArgs> WebStateChangeEvent;

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
        }

        public void VolumeChangeFromWeb(int encoder, int volume)
        {
            RaiseWebCommandEvent(new CommandFromWebEventArgs(encoder, WebCommandEnum.VOLCHANGE, false, volume, false, "", ""));
        }

        public void EncoderMutedChangeFromWeb(int encoder, bool muted)
        {
            RaiseWebCommandEvent(new CommandFromWebEventArgs(encoder, muted ? WebCommandEnum.MUTED : WebCommandEnum.UNMUTED, muted, 0, false, "", ""));
        }

        public void RebindController(int encoder, bool deviceBinding, string deviceID, string sessionID)
        {
            RaiseWebCommandEvent(new CommandFromWebEventArgs(encoder, WebCommandEnum.REBINDENCODER, false, 0, deviceBinding, deviceID, sessionID));
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
            if(e.EncoderNumber >= 1)
            {
                RaiseDeviceVolChangedEvent(e);
            }
        }
        public void SendMutedChangeEnocderNumber(DeviceVolChangedEventArgs e)
        {
            if(e.EncoderNumber >= 1)
            {
                RaiseDeviceVolChangedEvent(e);
            }
        }

        public void StopWeb()
        {
            _continue = false;
        }
    }

    public class SearchController : ApiController
    {
        [HttpGet]
        public string Search(string query)
        {
            return "Test";
        }
    }

    public class WebControlHub : Hub
    {
        private IWebConnector _webConnector;
        public WebControlHub(IWebConnector webCon)
        {
            _webConnector = webCon;
            _webConnector.DeviceVolChangedEvent += _webConnector_DeviceVolChangedEvent;
        }

        private void _webConnector_DeviceVolChangedEvent(object sender, DeviceVolChangedEventArgs e)
        {
            Clients.All.updateVol(e.EncoderNumber, e.Muted, e.Volume * 100);
        }

        public void ChangeVol(int enocder, int volume)
        {
            _webConnector.VolumeChangeFromWeb(enocder, volume);
        }

        public void MuteEncoder(int encoder, bool mute)
        {
            _webConnector.EncoderMutedChangeFromWeb(encoder, mute);
        }

        public void RebindEncoderToDevice(int encoder, string deviceID)
        {
            _webConnector.RebindController(encoder, true, deviceID, "");
        }

        public void RebindEncoderToSession(int encoder, string deviceID, string sessionID)
        {
            _webConnector.RebindController(encoder, false, deviceID, sessionID);
        }
    }
}
