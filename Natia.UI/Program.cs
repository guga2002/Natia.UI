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

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Starting Natia.UI application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHttpClient();
    builder.Services.AddCommonServices();

    builder.Services.AddScoped<ISolutionRecommendationService, SolutionRecommendationService>();
    builder.Services.AddScoped<ISmtpClientRepository, SmtpClientRepository>();
    builder.Services.AddScoped<INeuralMLPredict, NeuralMLPredict>();
    builder.Services.AddScoped<IImapServices, ImapService>();

    builder.Services.AddDbContext<SpeakerDbContext>(io =>
    {
        io.UseSqlServer(builder.Configuration.GetConnectionString("Server=192.168.1.102;Database=JandagBase;User Id=Guga13guga;Password=Guga13gagno!;Encrypt=True;TrustServerCertificate=True;"));
    });

    builder.Services.AddScoped<Main>();
    builder.Services.AddHostedService<Worker>();

    var app = builder.Build();
    logger.Info("Natia.UI application configured successfully. Starting host...");

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
