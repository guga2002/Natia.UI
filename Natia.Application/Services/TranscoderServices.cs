using Natia.Application.Contracts;
using Natia.Application.Dtos;
using Natia.Core.Entities;
using Natia.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Natia.Application.Services;

public class TranscoderServices : ITranscoderService
{
    private readonly ITranscoderRepository repos;
    private readonly ILogger<TranscoderServices> _logger;

    public TranscoderServices(ITranscoderRepository repos, ILogger<TranscoderServices> logger)
    {
        this.repos = repos;
        _logger = logger;
    }

    public async Task Add(TranscoderDto item)
    {
        try
        {
            _logger.LogInformation("Adding new Transcoder: Card={Card}, ChannelId={ChannelId}, Port={Port}",
                                   item.Card, item.ChanellId, item.Port);

            await repos.Add(new Transcoder()
            {
                Card = item.Card,
                ChanellId = item.ChanellId,
                Port = item.Port,
            });

            _logger.LogInformation("Successfully added Transcoder with Card={Card} and ChannelId={ChannelId}",
                                   item.Card, item.ChanellId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding Transcoder (Card={Card}, ChannelId={ChannelId})",
                             item.Card, item.ChanellId);
            throw;
        }
    }

    public async Task<int> GetChanellIdBycardandport(int card, int port)
    {
        try
        {
            _logger.LogInformation("Fetching ChannelId by Card={Card} and Port={Port}", card, port);
            var id = await repos.GetChanellIdByCardandPort(card, port);
            _logger.LogInformation("Fetched ChannelId={Id} for Card={Card}, Port={Port}", id, card, port);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ChannelId by Card={Card} and Port={Port}", card, port);
            throw;
        }
    }

    public async Task<TranscoderDto?> GetTranscoderInfoByChanellId(int id)
    {
        try
        {
            _logger.LogInformation("Fetching Transcoder info for ChannelId={ChannelId}", id);
            var res = await repos.GetTranscoderInfoByCHanellId(id);

            if (res != null)
            {
                _logger.LogInformation("Transcoder found: Card={Card}, ChannelId={ChannelId}, Port={Port}, EMR={EmrNumber}",
                                       res.Card, res.ChanellId, res.Port, res.EmrNumber);

                return new TranscoderDto()
                {
                    Card = res.Card,
                    ChanellId = res.ChanellId,
                    Port = res.Port,
                    Emr_Number = res.EmrNumber,
                };
            }

            _logger.LogWarning("No Transcoder info found for ChannelId={ChannelId}", id);
            return new TranscoderDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Transcoder info for ChannelId={ChannelId}", id);
            throw;
        }
    }

    public async Task Remove(int id)
    {
        try
        {
            _logger.LogInformation("Removing Transcoder with Id={Id}", id);
            await repos.Remove(id);
            _logger.LogInformation("Successfully removed Transcoder with Id={Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while removing Transcoder with Id={Id}", id);
            throw;
        }
    }

    public async Task View(int id)
    {
        try
        {
            _logger.LogWarning("View called, but currently it performs a Remove for Id={Id}. Check implementation!", id);
            await repos.Remove(id);
            _logger.LogInformation("Removed Transcoder via View() method for Id={Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing View() for Transcoder Id={Id}", id);
            throw;
        }
    }
}
