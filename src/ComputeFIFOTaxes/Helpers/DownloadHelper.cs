using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace ComputeFIFOTaxes.Helpers
{
    class DownloadHelper
    {
        public static string UrlEncode(Dictionary<string, string> args) => string.Join(
            "&",
            args.Where(x => x.Value != null).Select(x => x.Key + "=" + WebUtility.UrlEncode(x.Value))
        );

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Json result</returns>
        public static T Download<T>(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = client.GetAsync(url);
                response.Wait();
                var content = response.Result.Content.ReadAsStringAsync();
                content.Wait();

                return JsonConvert.DeserializeObject<T>(content.Result);
            }
        }
    }
}