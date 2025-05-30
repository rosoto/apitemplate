using NLog.Extensions.Logging;
using NLog.Web;
using Arquetipo.Api;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;

    logging.ClearProviders();

    if (environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase))
    {
        logging.AddConsole();
        logging.AddNLog();
    }
    else
    {
        logging.AddNLog();
    }
});

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

startup.Configure(app, app.Environment, provider);

app.Run();