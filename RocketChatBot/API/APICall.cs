using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace RocketChatBot.API
{
    class APICall
    {
        public static dynamic JSONPostRequest(string url, string jsonData, WebHeaderCollection headers = null)
        {
            dynamic response;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            if (headers != null)
                httpWebRequest.Headers = headers;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                response = JsonConvert.DeserializeObject(result);
            }
            return response;
        }

        public static dynamic AuthenticatedGETRequest(string url, WebHeaderCollection headers)
        {
            WebClient WC = new WebClient();
            WC.Headers = headers;
            dynamic returnVal = JsonConvert.DeserializeObject(WC.DownloadString(url));
            return returnVal;
        }
    }
}
