using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.Utils
{
    public class HttpProxy : IDisposable
    {
        private HttpClient _client;

        public HttpProxy()
        {
            var handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public string Get(string url)
        {
            var result = _client.GetStringAsync(url).Result;
            return result;
        }

        public string Post(string url, string body)
        {
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(body));
            var response = _client.PostAsync(url, content).Result;
            var result = response.Content.ReadAsStringAsync().Result;

            // check if right
            response.Dispose();
            return result;
        }

        public string Post(string url, object body) {
            var json = JsonHelper.Serialize(body);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json));

            var response = _client.PostAsync(url, content).Result;
            var result = response.Content.ReadAsStringAsync().Result;

            // check if right
            response.Dispose();
            return result;
        }
    }
}
