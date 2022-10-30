using ProjectMonitor.Shared;


#if DEBUG
ProjectMonitorConfig.SetupMonitor("debug", "");
#else
ProjectMonitorConfig.SetupMonitor("release", "");
#endif

ProjectMonitorCommon.ReportProjectUp("projectmonitor");

//await JobScheduler.SetupJobScheduler();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowUrls",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "https://dashboard.bregan.me");
                          policy.WithHeaders("Content-Type");
                          policy.WithMethods("GET", "POST", "DELETE");
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
