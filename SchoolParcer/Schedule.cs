using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule
{
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
                        lessonName = html.Substring(startSearchIndex, endSearchIndex - startSearchIndex),
                        Day = (Day)day,
                        classRoomNumber = null
                    };
                    endSearchIndex = html.IndexOf("</td>", endSearchIndex);
                    startSearchIndex = html.IndexOf("<p align=\"center\">", endSearchIndex);
                    endSearchIndex = html.IndexOf("</p>", startSearchIndex);
                    startSearchIndex += 18;
                    lesson.classRoomNumber = html.Substring(startSearchIndex, endSearchIndex - startSearchIndex);
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

        //public struct Cell
        //{
        //    public int startIndex { get; set; }
        //    public int endIndex { get; set; }

        //    public bool? isLessonName(string html)
        //    {
        //        var indexName = html.IndexOf("<p>", startIndex);
        //        var indexClassNumber = html.IndexOf("<p align=\"center\">", startIndex);

        //        if (indexName == -1 && indexClassNumber == -1)
        //        {
        //            return null;
        //        }
        //        else if (indexName == -1 && indexClassNumber != -1)
        //        {
        //            return false;
        //        }
        //        else if (indexName != -1 && indexClassNumber == -1)
        //        {
        //            return true;
        //        }

        //        return null;
        //    }
        //}
    }
}


//  <td style="width:162px;height:19px;">
//      <p>3.Английский язык</p>
//  </td>
//  <td style="width:47px;height:19px;">
//      <p align="center">404</p>
//      <p align="center">411</p>
//  </td>
