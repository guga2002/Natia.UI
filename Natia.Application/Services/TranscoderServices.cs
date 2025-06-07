using Natia.Application.Contracts;
using Natia.Application.Dtos;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Application.Services;

public class TranscoderServices : ITranscoderService
{
    private readonly ITranscoderRepository repos;
    public TranscoderServices(ITranscoderRepository repos)
    {
        this.repos = repos;
    }

    public async Task Add(TranscoderDto item)
    {
        await repos.Add(new Transcoder()
        {
            Card = item.Card,
            ChanellId = item.ChanellId,
            Port = item.Port,
        });
    }

    public async Task<int> GetChanellIdBycardandport(int card, int port)
    {
        return await repos.GetChanellIdByCardandPort(card, port);
    }

    public async Task<TranscoderDto?> GetTranscoderInfoByChanellId(int id)
    {
        var res = await repos.GetTranscoderInfoByCHanellId(id);
        if (res != null)
        {
            return new TranscoderDto()
            {
                Card = res.Card,
                ChanellId = res.ChanellId,
                Port = res.Port,
                Emr_Number = res.EmrNumber,
            };
        }
        return new TranscoderDto();
    }

    public async Task Remove(int id)
    {
        await repos.Remove(id);
    }

    public async Task View(int id)
    {
        await repos.Remove(id);
    }
}
