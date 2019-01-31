using System;
using System.IO;
using System.Net;
using System.Text;

namespace AbysmalLightScoreTracker
{
    public enum httpVerb
    {
        GET,
        POST
    }

    class RESTClient
    {
        public string URL { get; set; }
        public string header { get; set; }
        public httpVerb httpMethod { get; set; }

        public RESTClient()
        {
            URL = string.Empty;
            httpMethod = httpVerb.GET;
        }

        public string makeRequest()
        {
            string ResponseValue = string.Empty;

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
            myRequest.Headers.Add(header);
            myRequest.Method = httpMethod.ToString();

            using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                if(myResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("Error code" + myResponse.StatusCode.ToString());
                }

                using (Stream responseStream = myResponse.GetResponseStream())
                {
                    if(responseStream != null)
                    {
                        using (StreamReader Reader = new StreamReader(responseStream))
                        {
                            ResponseValue = Reader.ReadToEnd();
                        }
                    }
                }
            }

                return ResponseValue;
        }
    }
}
