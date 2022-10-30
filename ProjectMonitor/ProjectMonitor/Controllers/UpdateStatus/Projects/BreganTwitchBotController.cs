using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Helpers;

namespace ProjectMonitor.Api.Controllers.UpdateStatus.Projects
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreganTwitchBotController : ControllerBase
    {
        [HttpPost("UpdateUsersInStream")]
        public IActionResult UpdateUsersInStream([FromBody] long users)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().UsersInStream = users;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateTwitchIRCConnectionState")]
        public IActionResult UpdateTwitchIRCConnectionState([FromBody] bool status)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().TwitchIRCConnectionStatus = status;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateTwitchPubSubConnectionState")]
        public IActionResult UpdateTwitchPubSubConnectionState([FromBody] bool status)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().TwitchPubSubConnectionStatus = status;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateTwitchAPIKeyRefreshTime")]
        public IActionResult UpdateTwitchAPIKeyRefreshTime()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().TwitchApiKeyLastRefreshTime = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateDiscordBotConnectionState")]
        public IActionResult UpdateDiscordBotConnectionState([FromBody] bool status)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().DiscordConnectionStatus = status;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateStreamAnnounced")]
        public IActionResult UpdateStreamAnnounced([FromBody] bool status)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().StreamAnnounced = status;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateStreamStatus")]
        public IActionResult UpdateStreamStatus([FromBody] bool status)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().StreamStatus = status;

                if (status == true)
                {
                    context.BreganTwitchBot.Where(x => x.Mode == header).First().StreamLiveTime = DateTime.UtcNow;
                }

                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateStreamUptime")]
        public IActionResult UpdateStreamUptime([FromBody] TimeSpan time)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().StreamUptime = time;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateTwitchDailyPointsCollectingStatus")]
        public IActionResult UpdateTwitchDailyPointsCollectingStatus([FromBody] bool status)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().DailyPointsEnabled = status;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastHoursUpdateTime")]
        public IActionResult UpdateLastHoursUpdateTime()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().LastHoursUpdate = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastDiscordLeaderboardUpdate")]
        public IActionResult UpdateLastDiscordLeaderboardUpdate()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.BreganTwitchBot.Where(x => x.Mode == header).First().LastDiscordLeaderboardsUpdate = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }
    }
}
