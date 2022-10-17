using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Schedule
{
    public class ScheduleService : IScheduleService
    {
        private static HttpClient _client = new HttpClient();
        private const string _url = "https://sch2072v.mskobr.ru/uchashimsya/raspisanie-kanikuly";
        private List<Lesson> _lessons = new List<Lesson>();

        public List<Lesson> GetScheduleList(string className = null, Day? day = null)
        {
            return GetSchedule(className, day).ToList();
        }

        public void ScheduleUpdate()
        {
            _client.BaseAddress = new Uri(_url);
            string httpResult = GetHttpAsync().Result;
            Parser parser = new Parser();
            parser.onScheduleReady += () =>
            {
                _lessons = parser.lessons;
            };
            parser.ParseAll(GetHtmlForParse());
        }

        private async Task<string> GetHttpAsync()
        {
            HttpResponseMessage response = await _client.GetAsync(_url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        private string GetHtmlForParse()
        {
            string httpResult = GetHttpAsync().Result;
            var res = WebUtility.HtmlDecode(httpResult);
            return res;
        }

        public IEnumerable<Lesson> GetSchedule(string className = null, Day? day = null)
        {
            IEnumerable<Lesson> result = null;

            if (day == null && className != null)
            {
                result = _lessons
                    .Where(x => x.className.Equals(className));
            }
            else if (className == null && day != null)
            {
                result = _lessons
                    .Where(x => x.Day == day);
            }
            else if (day != null && className != null)
            {
                result = _lessons
                    .Where(x => x.className.Equals(className))
                    .Where(x => x.Day == day);
            }
            else
            {
                result = _lessons;
            }

            return result;
        }

        public string GetScheduleString(string className = null, Day? day = null)
        {
            string result = string.Empty;
            foreach (var lessonItem in _lessons)
            {
                result += $"{lessonItem.ToString()}\n";
            }
            return result;
        }
    }
}
