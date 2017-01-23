using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using OddsScraper.Contract.Model;
using OddsScraper;
using Tippspiel.Implementation;
using OddsScraper.Contract;

namespace BhFS.Tippspiel.Utils
{
    public class WettfreundeScraper
    {
        private readonly IOddsScraper _oddsScraper;

        public WettfreundeScraper(IOddsScraper oddsScraper)
        {
            _oddsScraper = oddsScraper;
        }

        public List<OddsInfoModel> Scrap(int spieltag)
        {
            string domainUrl = WettfreundeConfigInfo.Current.BaseURL;
            string betOddsUrl = domainUrl + WettfreundeConfigInfo.Current.OddsLink;

            CookieContainer cookies = new CookieContainer();

            var oddResponseStr = PostRequest("", betOddsUrl, "", cookies);

            var oddScraper = new WettfreundeOddsBuLiScraper();

            return oddScraper.GetOdds(oddResponseStr, spieltag.ToString());

        }

        string PostRequest(string domain, string url, string requestData, CookieContainer cookies)
        {
            Byte[] requestBytes = Encoding.UTF8.GetBytes(requestData);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //request.Proxy = new WebProxy("122.72.33.138:80");
            request.CookieContainer = cookies;
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestBytes.Length;
            request.AllowAutoRedirect = false;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:13.0) Gecko/20100101 Firefox/13.0.1";

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            while (response.StatusCode == HttpStatusCode.Found)
            {
                response.Close();

                String location = response.Headers[HttpResponseHeader.Location];

                request = WebRequest.Create(domain + location) as HttpWebRequest;
                request.CookieContainer = cookies;
                request.Method = WebRequestMethods.Http.Get;

                response = request.GetResponse() as HttpWebResponse;
            }

            StreamReader reader = new StreamReader(response.GetResponseStream());
            String responseData = reader.ReadToEnd();

            response.Close();

            return responseData;
        }
    }
}