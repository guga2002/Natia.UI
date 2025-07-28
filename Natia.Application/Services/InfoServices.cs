using Microsoft.Extensions.Logging;
using Natia.Application.Contracts;
using Natia.Application.Dtos;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Application.Services;

public class InfoServices : IInfoService
{
    private readonly IInfoRepository repos;
    private readonly ILogger<InfoServices> _logger;

    public InfoServices(IInfoRepository rep, ILogger<InfoServices> logger)
    {
        repos = rep;
        _logger = logger;
    }

    public async Task Add(InfoDto item)
    {
        try
        {
            _logger.LogInformation("Adding Info: ChanellId={ChanellId}, AlarmMessage='{Message}'", item.ChanellId, item.AlarmMessage);
            await repos.Add(new Infos()
            {
                AlarmMessage = item.AlarmMessage,
                CHanellId = item.ChanellId,
            });
            _logger.LogInformation("Successfully added Info for ChanellId={ChanellId}", item.ChanellId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add Info for ChanellId={ChanellId}", item.ChanellId);
            throw;
        }
    }

    public async Task<InfoDto?> GetInfoByChanellId(int id)
    {
        try
        {
            _logger.LogInformation("Fetching Info for ChanellId={ChanellId}", id);
            var res = await repos.GetInfoByChanellId(id);
            if (res is not null && res?.CHanellId > 0)
            {
                _logger.LogInformation("Found Info for ChanellId={ChanellId}", id);
                return new InfoDto()
                {
                    AlarmMessage = res?.AlarmMessage ?? "",
                    ChanellId = res?.CHanellId ?? 0,
                };
            }
            _logger.LogWarning("No Info found for ChanellId={ChanellId}", id);
            return new InfoDto()
            {
                AlarmMessage = ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Info for ChanellId={ChanellId}", id);
            throw;
        }
    }

    public async Task Remove(int id)
    {
        try
        {
            _logger.LogInformation("Removing Info with Id={Id}", id);
            await repos.Remove(id);
            _logger.LogInformation("Successfully removed Info with Id={Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove Info with Id={Id}", id);
            throw;
        }
    }

    public async Task View(int id)
    {
        try
        {
            _logger.LogInformation("Marking Info as viewed: Id={Id}", id);
            await repos.View(id);
            _logger.LogInformation("Successfully marked Info as viewed: Id={Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark Info as viewed: Id={Id}", id);
            throw;
        }
    }
}
