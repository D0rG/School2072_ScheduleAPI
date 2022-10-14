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
        internal const string _schoolName = "Расписание уроков обучающихся учебного корпуса № 3";

        static void Main(string[] args)
        {
            _client.BaseAddress = new Uri(_url);
            string httpResult = GetHttpAsync().Result;
            Console.WriteLine(httpResult + "\n" + "-----------------------------------------------------------------");

            Parse(GetHtmlForParse());
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

        private static void Parse(string httpResult)
        {
            var startTableIndex = httpResult.IndexOf(_schoolName);
            startTableIndex = httpResult.IndexOf("<tbody>", startTableIndex);
            var endTableIndex = httpResult.IndexOf("</tbody>", startTableIndex);
            Console.WriteLine($"start: {startTableIndex}; End: {endTableIndex};");
            RowCollection rowCollection = new RowCollection(httpResult, startTableIndex, endTableIndex);

            string currentClassName = string.Empty;
            Console.WriteLine(rowCollection.rows.Count);
            
            foreach (var lesson in rowCollection.GetLessons())
            {
                Console.WriteLine(lesson.ToString());
            }
            Console.WriteLine("End Parse");
        }

        private static string GetHtmlForParse(string school = _schoolName)
        {
            string httpResult = GetHttpAsync().Result;
            return WebUtility.HtmlDecode(httpResult);
        }
    }


}
