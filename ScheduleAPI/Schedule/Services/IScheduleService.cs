using System.Collections.Generic;

namespace Schedule
{
    public interface IScheduleService
    {
        public void ScheduleUpdate();
        public List<Lesson> GetSchedule();
        public IEnumerable<Lesson> GetSchedule(string className, Day? day);
    }
}
