using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Helpers;

namespace ProjectMonitor.Api.Controllers.UpdateStatus.Projects
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceManagerController : ControllerBase
    {
        [HttpPost("UpdateLastTransactionUpdate")]
        public IActionResult UpdateLastTransactionUpdate()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.FinanceManager.Where(x => x.Mode == header).First().LastTransactionUpdate = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastAPIRefresh")]
        public IActionResult UpdateLastAPIRefresh()
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.FinanceManager.Where(x => x.Mode == header).First().LastAPIRefresh = DateTime.UtcNow;
                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("UpdateLastAPIRefreshStatusCode")]
        public IActionResult UpdateLastAPIRefreshStatusCode([FromBody] string statusCode)
        {
            if (!ControllerHelper.ValidateApiRequest(Request.Headers))
            {
                return BadRequest();
            }

            string header = Request.Headers["EnvMode"];

            using (var context = new DatabaseContext())
            {
                context.FinanceManager.Where(x => x.Mode == header).First().LastAPIRefreshStatusCode = statusCode;
                context.SaveChanges();
            }

            return Ok();
        }
    }
}
