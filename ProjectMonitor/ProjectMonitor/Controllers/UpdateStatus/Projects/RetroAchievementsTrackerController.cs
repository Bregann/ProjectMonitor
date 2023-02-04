using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Helpers;

namespace ProjectMonitor.Api.Controllers.UpdateStatus.Projects
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetroAchievementsTrackerController : ControllerBase
    {
        [HttpPost("UpdateTotalGames")]
        public IActionResult UpdateTotalGames([FromBody] long games)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.RetroAchievementsTracker.Where(x => x.Mode == header).First().TotalGames = games;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateTotalRegisteredUsers")]
        public IActionResult UpdateTotalRegisteredUsers([FromBody] long users)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.RetroAchievementsTracker.Where(x => x.Mode == header).First().TotalUsers = users;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastGameUpdateTimeAndGameCountUpdated")]
        public IActionResult UpdateLastGameAndGamesUpdated([FromBody] long gamesUpdated)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.RetroAchievementsTracker.Where(x => x.Mode == header).First().LastGameUpdate = DateTime.UtcNow;
                context.RetroAchievementsTracker.Where(x => x.Mode == header).First().GamesUpdated = gamesUpdated;
                context.SaveChanges();
            }

            return Ok();
        }
    }
}