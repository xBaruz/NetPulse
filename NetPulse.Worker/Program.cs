using NetPulse.Application;
using NetPulse.Infrastructure;
using NetPulse.Infrastructure.Data;
using NetPulse.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "NetPulse Log Monitor";
});

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<LogWatcherWorker>();
builder.Services.AddHostedService<LogProcessorWorker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

await host.RunAsync();