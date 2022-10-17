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

        [HttpGet("/text")]
        public string GetScheduleString(string className = null, Day? day = null)
        {
            return Startup.scheduleService.GetScheduleString(className, day);
        }
    }
}
