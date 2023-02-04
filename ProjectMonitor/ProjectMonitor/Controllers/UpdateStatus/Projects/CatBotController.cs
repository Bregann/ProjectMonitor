using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Helpers;

namespace ProjectMonitor.Api.Controllers.UpdateStatus.Projects
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatBotController : ControllerBase
    {
        [HttpPost("UpdateDiscordConnectionState")]
        public IActionResult UpdateDiscordConnectionState([FromBody] bool state)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.CatBot.Where(x => x.Mode == header).First().DiscordConnectionStatus = state;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastTweetTime")]
        public IActionResult UpdateLastTweetTime()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.CatBot.Where(x => x.Mode == header).First().LastTweet = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastDiscordPostTime")]
        public IActionResult UpdateLastDiscordPostTime()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.CatBot.Where(x => x.Mode == header).First().LastDiscordPost = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }
    }
}