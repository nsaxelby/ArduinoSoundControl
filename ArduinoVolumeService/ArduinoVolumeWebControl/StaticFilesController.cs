using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ArduinoVolumeWebControl
{
    public class StaticFilesController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                url = "index.html";
            }

            var path = GeneratePath(url);
            var response = new HttpResponseMessage();
            if (File.Exists(path) == false)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }
            response.Content = new StringContent(File.ReadAllText(path));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(GetContentTypeFromExtension(url));
            return response;
        }

        private static string GeneratePath(string url)
        {
            string fileName = Path.GetFileName(url);
            return "StaticFiles/" + fileName;
        }

        private static string GetContentTypeFromExtension(string url)
        {
            string extension = Path.GetExtension(url);
            switch (extension.ToLower())
            {
                case ".json": return "text/javascript";
                case ".html": return "text/html";
                case ".css": return "text/css";
                default: return "text/plain";
            }
        }
    }
}
