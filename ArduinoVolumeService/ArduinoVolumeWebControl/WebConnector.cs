using ArduinoVolumeLib;
using System;
using System.Threading;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace ArduinoVolumeWebControl
{
    public class WebConnector
    {
        public bool _continue = true;
        public HttpSelfHostConfiguration _config;
        public Thread _runThread;
        public event EventHandler<WebConnectorStateChangeEventArgs> WebStateChangeEvent;
        private string _webUrlString;


        public WebConnector(string webUrl)
        {
            _webUrlString = webUrl;
            _config = new HttpSelfHostConfiguration(_webUrlString);

            _config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            _config.Routes.MapHttpRoute(
                "Static", "{*url}",
                new { controller = "StaticFiles", action = "Index" });
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

            using (HttpSelfHostServer server = new HttpSelfHostServer(_config))
            {
                Console.WriteLine("Starting web server");
                server.OpenAsync().Wait();
                Console.WriteLine("Started web server");
                RaiseStateChangeEvent(new WebConnectorStateChangeEventArgs("Started", WebStateEnum.Running, _webUrlString));
                while (_continue)
                {
                    Thread.Sleep(500);
                    if (_runThread.ThreadState != ThreadState.Running)
                    {
                        break;
                    }
                }
            }
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
}
