using System;
using System.Collections.Generic;

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
    }

    public class Schedule
    {
        public static Dictionary<Day, List<Lesson>> schoolSchedule;
    }

    public struct Row
    {
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public int lenght => endIndex - startIndex;

        public void Clear()
        {
            startIndex = -1;
            endIndex = -1;
        }

        public bool IsZero()
        {
            if (startIndex == 0 && endIndex == 0)
            {
                return true;
            }
            return false;
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
            className = className.Replace(" ", String.Empty);
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

        public List<Lesson> GetLessons(string className, string html)
        {
            var lessons = new List<Lesson>();
            int tryCount = ContainsClassName(html) ? 6 : 5;
            Lesson lesson = new Lesson();
            lesson.className = className;
            Cell temp = new Cell();

            var current
            do
            {
                temp.startIndex = html.IndexOf("<td>", currIndex);
                temp.endIndex = html.IndexOf("</td>", temp.startIndex);
                currIndex = temp.endIndex;
            }
            while (true);
        }
    }

    public struct Cell
    {
        public int startIndex {  get; set; }
        public int endIndex { get; set; }

        public bool? isLessonName(string html)
        {
            var indexName = html.IndexOf("<p>", startIndex);
            var indexClassNumber = html.IndexOf("<p align=\"center\">", startIndex);

            if (indexName == -1 && indexClassNumber == -1)
            {
                return null;
            }
            else if(indexName == -1 && indexClassNumber != -1)
            {
                return false;
            }
            else if (indexName != -1 && indexClassNumber == -1)
            {
                return true;
            }

            return null;
        }
    }
}


//  <td style="width:162px;height:19px;">
//      <p>3.Английский язык</p>
//  </td>
//  <td style="width:47px;height:19px;">
//      <p align="center">404</p>
//      <p align="center">411</p>
//  </td>
