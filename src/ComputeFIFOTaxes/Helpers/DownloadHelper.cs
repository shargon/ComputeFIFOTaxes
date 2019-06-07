using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace ComputeFIFOTaxes.Helpers
{
    public class DownloadHelper
    {
        static readonly string CacheFile = "downloadCache.json";
        static readonly IDictionary<string, string> _downloadCache = new Dictionary<string, string>();

        static DownloadHelper()
        {
            if (File.Exists(CacheFile))
            {
                // Load cache

                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(CacheFile));
                    if (data != null) _downloadCache = data;

#if DEBUG
                    Debugger.Log(0, "DownloadCache", "Loaded " + data.Values.Count + " entries");
#endif
                }
                catch { }
            }
        }

        public static string UrlEncode(Dictionary<string, string> args) => string.Join(
            "&",
            args.Where(x => x.Value != null).Select(x => x.Key + "=" + WebUtility.UrlEncode(x.Value))
        );

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="cached">Cached</param>
        /// <returns>Json result</returns>
        public static T Download<T>(string url, bool cached = true)
        {
            if (cached && _downloadCache.TryGetValue(url, out var cacheResult))
            {
                return JsonConvert.DeserializeObject<T>(cacheResult);
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = client.GetAsync(url);
                response.Wait();
                var content = response.Result.Content.ReadAsStringAsync();
                content.Wait();

                try
                {
                    return JsonConvert.DeserializeObject<T>(content.Result);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (cached)
                    {
                        _downloadCache[url] = content.Result;
                        File.WriteAllText(CacheFile, JsonConvert.SerializeObject(_downloadCache, Formatting.Indented));
                    }
                }
            }
        }
    }
}