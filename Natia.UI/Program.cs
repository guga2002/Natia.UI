using Microsoft.EntityFrameworkCore;
using Natia.Application.Contracts;
using Natia.Application.Services;
using Natia.Common.Services;
using Natia.Core.Context;
using Natia.Neurall.Interfaces;
using Natia.Neurall.Services;
using Natia.Persistance.Interface;
using Natia.Persistance.Repositories;
using Natia.UI.Jobs;
using NatiaGuard.BrainStorm.Main;
using NLog;
using NLog.Web;

var nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");

// Check if nlog.config exists before using it
if (!File.Exists(nlogConfigPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"FATAL: nlog.config not found at {nlogConfigPath}");
    Console.ResetColor();
    throw new FileNotFoundException($"Required logging configuration file not found: {nlogConfigPath}");
}

var logger = LogManager.Setup()
    .LoadConfigurationFromFile(nlogConfigPath)
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    builder.Host.UseWindowsService();

    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(1331);
    });

    builder.Services.AddHttpClient();
    builder.Services.AddCommonServices();

    builder.Services.AddScoped<ISolutionRecommendationService, SolutionRecommendationService>();
    builder.Services.AddScoped<ISmtpClientRepository, SmtpClientRepository>();
    builder.Services.AddScoped<INeuralMLPredict, NeuralMLPredict>();
    builder.Services.AddScoped<IImapServices, ImapService>();

    builder.Services.AddDbContext<SpeakerDbContext>(io =>
    {
        io.UseSqlServer(builder.Configuration.GetConnectionString(
            "Server=192.168.1.102;Database=JandagBase;User Id=Guga13guga;Password=Guga13gagno!;Encrypt=True;TrustServerCertificate=True;"
        ));
    });

    builder.Services.AddScoped<Main>();
    builder.Services.AddHostedService<Worker>();

    var app = builder.Build();
    logger.Info("Natia.UI application configured successfully. Starting host on port 1331...");

    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, "Application terminated unexpectedly.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
