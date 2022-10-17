using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Schedule; 

namespace ScheduleAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class ScheduleController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Lesson> GetSchedule(string className = null, Day? day = null)
        {
            return Startup.scheduleService.GetSchedule(className, day);
        }

        [HttpGet("/all")]
        public string GetScheduleString()
        {
            string result = string.Empty;
            foreach (var lessonItem in Startup.scheduleService.GetSchedule())
            {
                result += $"{lessonItem.ToString()}\n";
            }
            return result;
        }
    }
}
