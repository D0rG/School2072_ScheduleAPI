using System.Collections.Generic;

namespace Schedule
{
    public interface IScheduleService
    {
        public void ScheduleUpdate();

        public IEnumerable<Lesson> GetSchedule(string className, Day? day);

        public List<Lesson> GetScheduleList(string className, Day? day);

        public string GetScheduleString(string className = null, Day? day = null);
    }
}
