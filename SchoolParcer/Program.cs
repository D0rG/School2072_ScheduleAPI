using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Schedule;

namespace SchoolParcer
{
    internal class Program
    {
        internal static HttpClient _client = new HttpClient();
        internal const string _url = "https://sch2072v.mskobr.ru/uchashimsya/raspisanie-kanikuly";

        static void Main(string[] args)
        {
            _client.BaseAddress = new Uri(_url);
            string httpResult = GetHttpAsync().Result;
            Console.WriteLine(httpResult + "\n" + "-----------------------------------------------------------------");
            
            Parser parser = new Parser();
            parser.onScheduleReady += () =>
            {
                foreach (var lesson in parser.lessons)
                {
                    Console.WriteLine(lesson);
                }
            };
            parser.ParseAll(GetHtmlForParse());
            Console.ReadKey();
        }

        private static async Task<string> GetHttpAsync()
        {
            HttpResponseMessage response = await _client.GetAsync(_url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        private static string GetHtmlForParse()
        {
            string httpResult = GetHttpAsync().Result;
            return WebUtility.HtmlDecode(httpResult);
        }
    }
}
