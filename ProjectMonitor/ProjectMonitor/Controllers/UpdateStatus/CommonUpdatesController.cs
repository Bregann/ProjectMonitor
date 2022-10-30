using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Data;
using ProjectMonitor.Api.Data.Enums;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Database.Models;
using ProjectMonitor.Api.Models.UpdateStatus;

namespace ProjectMonitor.Api.Controllers.UpdateStatus
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonUpdatesController : ControllerBase
    {
        [HttpPost("AddAndUpdateProjectHealthStatus")]
        public IActionResult AddAndUpdateProjectHealthStatus(AddAndUpdateProjectHealthStatusDto dto)
        {
            if (Request.Headers["ProjectApiKey"].Count == 0)
            {
                return BadRequest();
            }

            string header = Request.Headers["ProjectApiKey"];

            //Make sure the API key is the same as the one in the API
            if (header != Config.Key)
            {
                return BadRequest();
            }

            using (var context = new DatabaseContext())
            {
                //Check if the project already exists
                var project = context.ProjectHealth.Where(x => x.ProjectName == dto.ProjectName).FirstOrDefault();

                //Add it if it doesn't exist
                if (project == null)
                {
                    context.ProjectHealth.Add(new ProjectHealth
                    {
                        ProjectName = dto.ProjectName,
                        ProjectRunning = true,
                        ProjectUptime = dto.ProjectUptime,
                        LastUpdate = DateTime.UtcNow,
                        CPUUsage = dto.CPUUsage,
                        RAMUsage = dto.RAMUsage
                    });
                }
                else
                {
                    //Check if the project went down by comparing the uptime
                    if (dto.ProjectUptime < project.ProjectUptime)
                    {
                        ErrorMonitoring.AddNewError(ErrorTypes.ProjectCrashed.ToString(), "Project crashed unexpectedly", dto.ProjectName);
                    }

                    project.ProjectUptime = dto.ProjectUptime;
                    project.ProjectRunning = true;
                    project.LastUpdate = DateTime.UtcNow;
                    project.CPUUsage = dto.CPUUsage;
                    project.RAMUsage = dto.RAMUsage;

                    context.ProjectHealth.Update(project);
                }

                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("AddAndUpdateSystemData")]
        public IActionResult AddAndUpdateSystemData(AddAndUpdateSystemDataDto dto)
        {
            if (Request.Headers["ProjectApiKey"].Count == 0)
            {
                return BadRequest();
            }

            string header = Request.Headers["ProjectApiKey"];

            //Make sure the API key is the same as the one in the API
            if (header != Config.Key)
            {
                return BadRequest();
            }

            using (var context = new DatabaseContext())
            {
                var system = context.SystemHealth.Where(x => x.SystemName == dto.SystemName).FirstOrDefault();

                //Add it if it doesn't exist
                if (system == null)
                {
                    context.SystemHealth.Add(new SystemHealth
                    {
                        SystemName = dto.SystemName,
                        SystemUptime = dto.SystemUptime,
                        LastUpdate = DateTime.UtcNow,
                    });
                }
                else
                {
                    system.SystemUptime = dto.SystemUptime;
                    system.LastUpdate = DateTime.UtcNow;

                    context.SystemHealth.Update(system);
                }

                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("SendAlert")]
        public IActionResult SendAlert([FromBody] string message)
        {
            return Ok();
        }
    }


}
