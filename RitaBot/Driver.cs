using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using TravianTools.SeleniumHost;
using Cookie = System.Net.Cookie;
using DataFormat = RestSharp.DataFormat;

namespace RitaBot
{
    public class Driver : NotificationObject
    {
        private SeleniumHostWPF _host;

        public ChromeDriverService Service { get; set; }
        private ChromeOptions Options { get; set; }
        public ChromeDriver Chrome { get; set; }
        public IJavaScriptExecutor JsExec { get; set; }
        public Actions Act { get; set; }

        public SeleniumHostWPF Host
        {
            get => _host;
            set { _host = value; RaisePropertyChanged(() => Host);}
        }

        //public RestClientOptions RestOptions { get; set; }
        //public RestClient RestClient { get; set; }

        public void Init()
        {
            if (!Directory.Exists($"{g.UserDataPath}"))
                Directory.CreateDirectory($"{g.UserDataPath}");
            Logger.Info($"Start driver initialization");
            Service = ChromeDriverService.CreateDefaultService();
            Service.HideCommandPromptWindow = true;
            Options = new ChromeOptions();

            Options.AddArgument($"user-data-dir={g.UserDataPath}");
            Chrome = new ChromeDriver(Service, Options);
            JsExec = Chrome;
            Act = new Actions(Chrome);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Host = new SeleniumHostWPF
                {
                    DriverService = Service
                };
            });

            //RestOptions = new RestClientOptions("https://auction.tdera.ru/")//api/staffRequests?filter%5BdateFrom%5D=2022-04-07T00%3A00%3A00&filter%5BdateTo%5D=2022-05-22T00%3A00%3A00
            //{
            //    UserAgent = JsExec.ExecuteScript("return navigator.userAgent").ToString()
            //    //UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.109 Safari/537.36 OPR/84.0.4316.52"
            //};
            //RestClient = new RestClient(RestOptions);
            Logger.Info($"End driver initialization");
        }

        public void DeInit()
        {
            Logger.Info($"Start driver deinitialization");
            Host.DriverService = null;
            Host = null;
            Act = null;
            JsExec = null;
            Chrome.Dispose();
            Chrome = null;
            Options = null;
            Service.Dispose();
            Service = null;
            Logger.Info($"End driver deinitialization");
        }

        public string GetCookieString() => Chrome.Manage().Cookies.AllCookies.Aggregate("", (s, c) => $"{s}{c.Name}={c.Value};");

        public IWebElement Wait(By by, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(Chrome, TimeSpan.FromSeconds(timeout));
                var el = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

                Logger.Info($"Browser Wait element={by} timeout={timeout} succ");
                return el;
            }
            catch
            {
                Logger.Info($"Browser Wait element={by} timeout={timeout} err");
                return null;
            }
        }
        
        public bool IsBrowserClosed()
        {
            var isClosed = false;
            try
            {
                _ = Chrome.Title;
            }
            catch
            {
                isClosed = true;
            }

            return isClosed;
        }




        //public dynamic Post()
        //{
        //    try
        //    {
        //        var dt  = DateTime.Now;
        //        var req = new RestRequest($"/api/staffRequests?filter%5BdateFrom%5D={dt.Year}-{dt.Month:00}-{dt.Day:00}T00%3A00%3A00&filter%5BdateTo%5D=2022-05-22T00%3A00%3A00");
        //        //req.AddParameter("filter[dateFrom]", $"{dt.Year}-{dt.Month:00}-{dt.Day:00}T00:00:00");
        //        //req.AddParameter("filter[dateTo]", $"{dt.Year}-05-22T00:00:00");
        //        //req.AddParameter("a", (string)(json as dynamic).action.ToString(), ParameterType.QueryString);
        //        //req.AddParameter("t", GetTimeStamp(), ParameterType.QueryString);
        //        //var data = Rem(json.ToString());
        //        //var buffer = Encoding.UTF8.GetBytes(data);
        //        req.AddHeader("Accept",             "application/json, text/plain, */*");
        //        req.AddHeader("Accept-Encoding",    "gzip, deflate, br");
        //        req.AddHeader("Accept-Language",    "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
        //        req.AddHeader("Referer",            "https://auction.tdera.ru/");
        //        req.AddHeader("sec-ch-ua",          " Not A;Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Google Chrome\";v=\"100");
        //        req.AddHeader("sec-ch-ua-mobile",   "?0");
        //        req.AddHeader("sec-ch-ua-platform", "Windows");
        //        req.AddHeader("sec-fetch-dest",     "empty");
        //        req.AddHeader("sec-fetch-mode",     "cors");
        //        req.AddHeader("sec-fetch-site",     "same-origin");
        //        req.AddHeader("authority", "auction.tdera.ru");
        //        req.AddHeader("method", "GET");
        //        req.AddHeader("path", $"/api/staffRequests?filter%5BdateFrom%5D={dt.Year}-{dt.Month:00}-{dt.Day:00}T00%3A00%3A00&filter%5BdateTo%5D=2022-05-22T00%3A00%3A00");
        //        req.AddHeader("scheme", "https");
        //        req.AddHeader("cookie",             GetCookieString());
        //        var cookies = Chrome.Manage().Cookies.AllCookies;

        //        RestOptions.CookieContainer = new CookieContainer();
        //        foreach (var cookie in cookies)
        //            RestOptions.CookieContainer?.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));

        //        var res = RestClient.ExecuteAsync(req).GetAwaiter().GetResult();
        //        Logger.Data(res.Content);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Info(e.ToString());
        //    }

        //    return null;
        //}

        public dynamic Get()
        {
            try
            {

                var dt = DateTime.Now;
                var req =
                    (HttpWebRequest)
                    WebRequest.Create($"https://auction.tdera.ru/api/staffRequests?filter%5BdateFrom%5D={dt.Year}-{dt.Month:00}-{dt.Day:00}T00%3A00%3A00&filter%5BdateTo%5D={dt.Year}-{dt.Month+2:00}-{dt.Day:00}T00%3A00%3A00");

                req.Method    = "GET";
                req.Accept    = "application/json, text/plain, */*";
                req.Referer   = $"https://auction.tdera.ru/";
                req.UserAgent = JsExec.ExecuteScript("return navigator.userAgent").ToString();

                req.Headers.Add("Accept-Encoding",    "gzip, deflate, br");
                req.Headers.Add("Accept-Language",    "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req.Headers.Add("sec-ch-ua",          " Not A;Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Google Chrome\";v=\"100");
                req.Headers.Add("sec-ch-ua-mobile",   "?0");
                req.Headers.Add("sec-ch-ua-platform", "Windows");
                req.Headers.Add("sec-fetch-dest",     "empty");
                req.Headers.Add("sec-fetch-mode",     "cors");
                req.Headers.Add("sec-fetch-site",     "same-origin");

                req.Headers.Add("Cookie", GetCookieString());
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                var resp        = (HttpWebResponse)req.GetResponse();
                var strReader   = new StreamReader(resp.GetResponseStream());
                var workingPage = strReader.ReadToEnd();
                Logger.Data(workingPage);
                var jo          = JArray.Parse(workingPage) as dynamic;
                resp.Close();
                return jo;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            return null;
        }

        public dynamic GetFavs()
        {
            try
            {

                var dt = DateTime.Now;
                var req =
                    (HttpWebRequest)
                    WebRequest.Create($"https://auction.tdera.ru/api/shops/favorites");

                req.Method    = "GET";
                req.Accept    = "application/json, text/plain, */*";
                req.Referer   = $"https://auction.tdera.ru/";
                req.UserAgent = JsExec.ExecuteScript("return navigator.userAgent").ToString();

                req.Headers.Add("Accept-Encoding",    "gzip, deflate, br");
                req.Headers.Add("Accept-Language",    "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req.Headers.Add("sec-ch-ua",          " Not A;Brand\";v=\"99\", \"Chromium\";v=\"100\", \"Google Chrome\";v=\"100");
                req.Headers.Add("sec-ch-ua-mobile",   "?0");
                req.Headers.Add("sec-ch-ua-platform", "Windows");
                req.Headers.Add("sec-fetch-dest",     "empty");
                req.Headers.Add("sec-fetch-mode",     "cors");
                req.Headers.Add("sec-fetch-site",     "same-origin");

                req.Headers.Add("Cookie", GetCookieString());
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                var resp        = (HttpWebResponse)req.GetResponse();
                var strReader   = new StreamReader(resp.GetResponseStream());
                var workingPage = strReader.ReadToEnd();
                Logger.Data(workingPage);
                var jo          = JArray.Parse(workingPage) as dynamic;
                resp.Close();
                return jo;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            return null;
        }
    }
}
