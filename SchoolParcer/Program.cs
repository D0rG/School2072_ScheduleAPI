using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine("Start Parse");
            Parse();
            Debug.WriteLine("End Parse");
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

        private static void Parse(string school = _schoolName, string className = "5«К»")
        {
            string httpResult = GetHttpAsync().Result;
            httpResult = WebUtility.HtmlDecode(httpResult);
            var startTableIndex = httpResult.IndexOf(school);
            var endTableIndex = httpResult.IndexOf("</table>", startTableIndex);

            List<Row> rows = GetRows(httpResult, startTableIndex, endTableIndex);

            //int count = 0;
            //foreach (Row row in rows)
            //{
            //    if (row.ContainsClassName(httpResult))
            //    {
            //        count++;
            //    }
            //}
            //Console.WriteLine($"Rows with class name = {count};");  //Class count
            //Console.WriteLine($"Rows Count = {rows.Count};");

            string currentClassName = string.Empty;
            foreach (Row row in rows)
            {
                if(row.ContainsClassName(httpResult, out string newClassName))
                {
                    currentClassName = newClassName;
                    Console.WriteLine(currentClassName);
                    //Lesson lesson = new Lesson
                    //{
                    //    lessonName = "",
                    //    className = currentClassName,
                    //    classRoomNumbers = new List<int> { 1, 2 }
                    //};
                }
            }
        }

        private static List<Row> GetRows(string html, int start, int end)
        {
            var rows = GetAllRows(html);
            var temp = new List<Row>();

            for (int i = 1; i < rows.Count; i++)
            {
                if (rows[i].startIndex >= start && rows[i].endIndex <= end )
                {
                    temp.Add(rows[i]);
                }
            }

            if (temp.Count > 0) { temp.RemoveAt(0); }   //Удяляем первый элемент, т.к. в нем только дни недели.
            return temp;
        }

        private static List<Row> GetAllRows(string html)
        {
            List<Row> rows = new List<Row>();
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            int currIndex = 0;
            Row temp = new Row();

            do
            {
                if (!temp.IsZero())
                {
                    rows.Add(temp);
                }

                try
                {
                    temp.startIndex = html.IndexOf("<tr>", currIndex);
                    temp.endIndex = html.IndexOf("</tr>", temp.startIndex);
                    currIndex = temp.endIndex;
                }
                catch
                {
                    break;
                }
            }
            while (temp.startIndex != -1);

            return rows;
        }
    }


}
