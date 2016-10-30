using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace rupload.Helpers
{
    public class Request
    {
        public static async Task<T> PostAsync<T>(string url, string serializedBody)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                using (HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(serializedBody)))
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseString);
                }
            }
        }

        public static async Task<T> ShortenUrl<T>(object bodyObject)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                httpClient.BaseAddress = new Uri("http://ouo.press");
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/shorten"))
                {
                    request.Content = new FormUrlEncodedContent(ObjectToKeyValuePair(bodyObject));
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(responseString);
                    }
                }
            }
        }

        static IEnumerable<KeyValuePair<string, string>> ObjectToKeyValuePair(object obj)
        {
            var props = obj.GetType().GetProperties();
            foreach (var property in props)
            {
                yield return new KeyValuePair<string, string>(property.Name, property.GetValue(obj).ToString());
            }
        }
    }
}
