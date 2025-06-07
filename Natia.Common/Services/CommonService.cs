using Microsoft.Extensions.DependencyInjection;
using Natia.Core.Interfaces;
using Natia.Core.Repositories;
using Natia.Persistance.Interface;
using Natia.Persistance.Repositories;
using Natia.Application.Contracts;
using Natia.Application.Services;

namespace Natia.Common.Services;

public static class CommonService
{
    public static void AddCommonServices(this IServiceCollection service)
    {
        service.AddScoped<IChanellRepository, ChanellRepository>();

        service.AddScoped<IDesclamblerRepository, DesclamblerRepository>();

        service.AddScoped<IEmr60InfoRepository, Emr60InfoRepository>();

        service.AddScoped<IInfoRepository, InfoRepository>();

        service.AddScoped<IRecieverRepository, RecieverRepository>();

        service.AddScoped<ITranscoderRepository, TranscoderReporitory>();

        service.AddScoped<ISoundRepository, SoundRepository>();

        service.AddScoped<IAllinOneService, AllInOneService>();

        service.AddScoped<IAzureSpeechToTextService, AzureSpeechToTextService>();

        service.AddScoped<IChanellService, ChanellServices>();

        service.AddScoped<IInfoService, InfoServices>();

        service.AddScoped<ISoundService, SounServices>();

        service.AddScoped<ITempperatureService, TemperatureService>();

        service.AddScoped<ITranscoderService, TranscoderServices>();

        service.AddScoped<PortCheckAndRefresh>();

        service.AddScoped<CheckFromWhereItIsCameFrom>();

        service.AddScoped<INeuralRepository, NeurallRepository>();

        service.AddScoped<IChanellRepository, ChanellRepository>();
    } 
}
