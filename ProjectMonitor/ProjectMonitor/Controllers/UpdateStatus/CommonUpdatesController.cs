using Microsoft.AspNetCore.Mvc;
using ProjectMonitor.Api.Data;
using ProjectMonitor.Api.Data.Enums;
using ProjectMonitor.Api.Database.Context;
using ProjectMonitor.Api.Database.Models;
using ProjectMonitor.Api.Dtos.RequestDtos.UpdateStatus;
using Serilog;

namespace ProjectMonitor.Api.Controllers.UpdateStatus
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonUpdatesController : ControllerBase
    {
        [HttpPost("AddAndUpdateProjectHealthStatus")]
        public IActionResult AddAndUpdateProjectHealthStatus(AddAndUpdateProjectHealthStatusRequestDto requestDto)
        {
            string apiKey = Request.Headers.Authorization;

            if (apiKey == null || apiKey != AppConfig.ApiKey)
            {
                Log.Information($"[Project Monitor Api] AddAndUpdateProjectHealthStatus - request missing api key or is invalid");
                return BadRequest();
            }

            using (var context = new DatabaseContext())
            {
                //Check if the project already exists
                var project = context.ProjectHealth.Where(x => x.ProjectName == requestDto.ProjectName).FirstOrDefault();

                //Add it if it doesn't exist
                if (project == null)
                {
                    context.ProjectHealth.Add(new ProjectHealth
                    {
                        ProjectName = requestDto.ProjectName,
                        ProjectRunning = true,
                        ProjectUptime = requestDto.ProjectUptime,
                        LastUpdate = DateTime.UtcNow,
                        CPUUsage = requestDto.CPUUsage,
                        RAMUsage = requestDto.RAMUsage
                    });

                    Log.Information($"[Project Monitor Api] AddAndUpdateProjectHealthStatus - project {requestDto.ProjectName} added");
                }
                else
                {
                    //Check if the project went down by comparing the uptime
                    if (requestDto.ProjectUptime.TotalSeconds < 60)
                    {
                        Log.Information($"[Project Monitor Api] AddAndUpdateProjectHealthStatus - Project crash detected in {project.ProjectName}- old uptime: {project.ProjectUptime} new uptime: {requestDto.ProjectUptime}");
                        ErrorMonitoring.AddNewError(ErrorTypes.ProjectCrashed.ToString(), "Project crashed unexpectedly", requestDto.ProjectName);
                    }

                    project.ProjectUptime = requestDto.ProjectUptime;
                    project.ProjectRunning = true;
                    project.LastUpdate = DateTime.UtcNow;
                    project.CPUUsage = requestDto.CPUUsage;
                    project.RAMUsage = requestDto.RAMUsage;

                    context.ProjectHealth.Update(project);
                }

                context.SaveChanges();
                Log.Information($"[Project Monitor Api] AddAndUpdateProjectHealthStatus - project {requestDto.ProjectName} updated");
            }

            return Ok();
        }

        [HttpPost("AddAndUpdateSystemData")]
        public IActionResult AddAndUpdateSystemData(AddAndUpdateSystemDataRequestDto requestDto)
        {
            string apiKey = Request.Headers.Authorization;

            if (apiKey == null || apiKey != AppConfig.ApiKey)
            {
                Log.Information($"[Project Monitor Api] AddAndUpdateSystemData - request missing api key or is invalid");
                return BadRequest();
            }

            using (var context = new DatabaseContext())
            {
                var system = context.SystemHealth.Where(x => x.SystemName == requestDto.SystemName).FirstOrDefault();

                //Add it if it doesn't exist
                if (system == null)
                {
                    context.SystemHealth.Add(new SystemHealth
                    {
                        SystemName = requestDto.SystemName,
                        SystemUptime = requestDto.SystemUptime,
                        LastUpdate = DateTime.UtcNow,
                    });

                    Log.Information($"[Project Monitor Api] AddAndUpdateSystemData - new system added {requestDto.SystemName}");
                }
                else
                {
                    system.SystemUptime = requestDto.SystemUptime;
                    system.LastUpdate = DateTime.UtcNow;
                    context.SystemHealth.Update(system);
                }

                context.SaveChanges();
                Log.Information($"[Project Monitor Api] AddAndUpdateSystemData - system updated {requestDto.SystemName}");
            }

            return Ok();
        }
    }
}