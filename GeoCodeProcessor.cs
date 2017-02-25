using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Core.Dispatcher;
using Data.Interfaces;
using NLog;

namespace api.Core
{
    public class GeoCodeProcessor
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public async Task<bool> Process(IEnumerable<IWorkItem> items, IRateLimiterRepository repo, IDispatcherValidateProgressBatch progressDispatcher)
        {
            try {                        
                var rnd = new Random();
                const int MaxAttempts = 10;
                foreach (var item in items)
                {
                    int i = 0;
                    item.Result = await GetResult(item);
                    while (item.Result.Contains("OVER_QUERY_LIMIT") && i < MaxAttempts)
                    {
                        item.Result = await GetResult(item);
                        await Task.Delay(rnd.Next(850, 1000));
                        i++;
                        _logger.Debug(string.Format("OVER_QUERY_LIMIT, attempt: {0}", i));
                    }
                    if (i < MaxAttempts) await repo.SaveResult(item);

                    if (progressDispatcher != null) // avoid the case where there is no hub at all
                    {
                        await progressDispatcher.Send(new MessageProgressBatch { ID = item.HydrantId }, new Task(() => { }));
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error Task Process: ", ex);
            }

            return true;
        }

        private async Task<string> GetResult(IWorkItem item)
        {
            var content = new MemoryStream();
            try {                        
                    if (string.IsNullOrWhiteSpace(item.WorkItem))
                    {
                        if (!string.IsNullOrWhiteSpace(item.Address))
                        {
                            item.WorkItem = string.Format("address={0}", Clean(item.Address));
                        }
                    }

                    var url = GenerateGoogleGeoCodeUrl(item.WorkItem, true, item.Type == "json");
                    var webRequest = (HttpWebRequest)WebRequest.Create(url);

                    using (WebResponse response = await webRequest.GetResponseAsync())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null) await responseStream.CopyToAsync(content);
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error GenerateGoogleGeoCodeUrl: ", ex);
            }
            return Encoding.UTF8.GetString(content.GetBuffer(), 0, (int)content.Length);
        }

        private static string  GenerateGoogleGeoCodeUrl(string request, bool isSecure, bool json)
        {
            var myOwnQs = request;
            var httpOrHttps = isSecure ? "https" : "http";
               
            var clientParam =
             ConfigurationManager.AppSettings["Google.Maps.UseClientId"] == "Yes"
                 ? string.Format("&client={0}",
                                 ConfigurationManager.AppSettings["Google.Maps.ClientID"] ?? "gme-emergencyservices")
                 : "";

            string url = json ? string.Format("{0}://maps.googleapis.com/maps/api/geocode/json?{1}{2}",
                                       httpOrHttps, myOwnQs, clientParam)
                                : string.Format("{0}://maps.googleapis.com/maps/api/geocode/xml?{1}{2}",
                                       httpOrHttps, myOwnQs, clientParam);

            if (ConfigurationManager.AppSettings["Google.Maps.UseClientId"] == "Yes")
            {
                var keyString = ConfigurationManager.AppSettings["Google.Maps.Key"];

                var encoding = new ASCIIEncoding();

                // converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
                string usablePrivateKey = keyString.Replace("-", "+").Replace("_", "/");
                byte[] privateKeyBytes = Convert.FromBase64String(usablePrivateKey);

                var uri = new Uri(url);
                byte[] encodedPathAndQueryBytes = encoding.GetBytes(uri.LocalPath + uri.Query);

                // compute the hash
                var algorithm = new HMACSHA1(privateKeyBytes);
                byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

                // convert the bytes to string and make url-safe by replacing '+' and '/' characters
                string signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

                // Add the signature to the existing URI.
                return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
            }

            return url;
        }

        private string Clean(string url)
        {
            StringBuilder sb = new StringBuilder(url);
            // Reserved Characters
            // RFC 3986
            // gen-delims 
            sb.Replace(":", "");
            sb.Replace("/", "");
            sb.Replace("?", "");
            sb.Replace("#", "");
            sb.Replace("[", "");
            sb.Replace("]", "");
            sb.Replace("@", "");
            // sub-delims 
            sb.Replace("!", "");
            sb.Replace("$", "");
            sb.Replace("&", "");
            sb.Replace("'", "");
            sb.Replace("(", "");
            sb.Replace(")", "");
            sb.Replace("*", "");
            sb.Replace("+", "");
            sb.Replace(",", "");
            sb.Replace(";", "");
            sb.Replace("=", "");
            return sb.ToString();
        }
    }
}