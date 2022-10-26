using Microsoft.AspNetCore.Mvc;
using Schedule;
using System.Linq;

namespace ScheduleAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class ScheduleController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSchedule(string className = null, Day? day = null)
        {
            var result = Startup.scheduleService.GetSchedule(className, day).ToList<Lesson>();
            if (result.Count == 0)  {   return NotFound();  }
            return Ok(result);
        }

        [HttpGet("/text")]
        public IActionResult GetScheduleString(string className = null, Day? day = null)
        {
            var result = Startup.scheduleService.GetScheduleString(className, day);
            if (string.IsNullOrWhiteSpace(result))  {   return NotFound();  }
            return Ok(result);
        }
    }
}
