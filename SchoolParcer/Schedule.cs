using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule
{
    public sealed class Parser
    {
        private string[] _schoolNames =
        {
            "Расписание уроков обучающихся учебного корпуса № 3",
            "Расписание уроков обучающихся учебного корпуса № 2",
            "Расписание уроков обучающихся учебного корпуса № 1"
        };
        private string httpResult = string.Empty;
        private List<Lesson> _lessons = new List<Lesson>();
        public List<Lesson> lessons { get { return _lessons; } }

        public Action onScheduleReady;

        internal void ParseAll(string httpResult)
        {
            this.httpResult = httpResult;
            foreach (string schoolName in _schoolNames)
            {
                Parse(schoolName);
            }
            Console.WriteLine("Ready!");
            onScheduleReady.Invoke();
        }

        private void Parse(string schoolName)
        {
            var startTableIndex = httpResult.IndexOf(schoolName);
            startTableIndex = httpResult.IndexOf("<tbody>", startTableIndex);
            var endTableIndex = httpResult.IndexOf("</tbody>", startTableIndex);
            RowCollection rowCollection = new RowCollection(httpResult, startTableIndex, endTableIndex);
            foreach (var lesson in rowCollection.GetLessons())
            {
                _lessons.Add(lesson);
            }
        }
    }

    public class Schedule
    {
        public static Dictionary<Day, List<Lesson>> schoolSchedule;
    }

    public class RowCollection
    {
        private string html;
        public List<Row> rows = new List<Row>();
        public int Count => rows.Count;

        public RowCollection(string html, int start, int end)
        {
            this.html = html;
            rows = GetAllRows(html).Where(x => (x.startIndex >= start && x.startIndex <= end)).ToList();
            if (rows.Count > 0) { rows.RemoveAt(0); }   //Удяляем первый элемент, т.к. в нем только дни недели.
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
                if (!temp.isZero)
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

        public List<Lesson> GetLessons()
        {
            var lessons = new List<Lesson>();

            string className = string.Empty;
            foreach (var row in rows)
            {
                if (row.ContainsClassName(html, out string newClassName))
                {
                    className = newClassName;
                }

                foreach (var lesson in row.GetRowLessons(html))
                {
                    var temp = lesson;
                    temp.className = new string(className);
                    lessons.Add(temp);
                }
            }
            return lessons.Where(x => !string.IsNullOrEmpty(x.lessonName.Trim())).ToList(); //Отчиста пустых уроков.
        }
    }

    public struct Row
    {
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public int lenght => endIndex - startIndex;
        public bool isZero => startIndex == 0 && endIndex == 0;

        public void Clear()
        {
            startIndex = -1;
            endIndex = -1;
        }

        public bool ContainsClassName(string html, out string className)
        {
            className = null;
            int index = -1;

            try
            {
                index = html.IndexOf("<strong>", startIndex);
            }
            catch
            {
                ;
            }

            if (index == -1 || index > endIndex)
            {
                return false;
            }

            index += 8;
            int endNameIndex = html.IndexOf("</strong>", index);
            className = html.Substring(index, endNameIndex - index);
            className = className.Replace(" ", string.Empty);
            className = className.Replace("»", string.Empty);
            className = className.Replace("«", string.Empty);

            return true;
        }

        public bool ContainsClassName(string html)
        {
            int index = -1;
            try
            {
                index = html.IndexOf("<strong>", startIndex);
            }
            catch
            {
                ;
            }

            if (index == -1 || index > endIndex)
            {
                return false;
            }
            return true;
        }

        public List<Lesson> GetRowLessons(string html)
        {
            List<Lesson> result = new List<Lesson>();
            html = html.Substring(this.startIndex, lenght);

            int day = 0;
            int startSearchIndex = 0;
            int endSearchIndex = 0;
            while (true)
            {
                try
                {
                    startSearchIndex = html.IndexOf("<td style", endSearchIndex); //Пропускаем имя класса. 
                    startSearchIndex = html.IndexOf("<p>", startSearchIndex);
                    endSearchIndex = html.IndexOf("</p>", startSearchIndex);
                    startSearchIndex += 3;
                    Lesson lesson = new Lesson()
                    {
                        className = string.Empty,
                        lessonName = html.Substring(startSearchIndex, endSearchIndex - startSearchIndex).Trim(),
                        Day = (Day)day,
                        classRoomNumber = null
                    };
                    endSearchIndex = html.IndexOf("</td>", endSearchIndex);

                    var htmlSub = html.Substring(endSearchIndex + 5, html.IndexOf("</td>", endSearchIndex + 4) - endSearchIndex);
                    int searchRoomIndexStart = 0;
                    int searchRoomIndexEnd = 0;
                    while (true)
                    {
                        try
                        {
                            searchRoomIndexStart = htmlSub.IndexOf("<p align=\"center\">", searchRoomIndexEnd);
                            searchRoomIndexEnd = htmlSub.IndexOf("</p>", searchRoomIndexStart);
                            searchRoomIndexStart += 18;
                        }
                        catch
                        {
                            break;
                        }

                        lesson.classRoomNumber += htmlSub.Substring(searchRoomIndexStart, searchRoomIndexEnd - searchRoomIndexStart) + " ";
                    }


                    result.Add(lesson);
                    day++;
                }
                catch
                {
                    break;
                }

            }
            return result;
        }
    }

    public struct Lesson
    {
        public Day Day { get; set; }
        public string className { get; set; }
        public string lessonName { get; set; }
        public string classRoomNumber { get; set; }

        public override string ToString()
        {
            return $"Класс: {className}, урок: {lessonName}, Кабинет: {classRoomNumber}, День: {Day}";
        }
    }

    public enum Day
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday,
    }
}
