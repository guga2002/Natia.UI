using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories;

public class TranscoderReporitory : BaseRepository<Transcoder>, ITranscoderRepository
{
    public TranscoderReporitory(SpeakerDbContext context) : base(context)
    {
    }

    public async Task<int> GetChanellIdByCardandPort(int card, int port)
    {
        var res = await _mainSet.FirstOrDefaultAsync(io => io.Card == card && io.Port == port);
        if (res != null)
        {
            return res.ChanellId;
        }
        return -1;
    }

    public async Task<Transcoder?> GetTranscoderInfoByCHanellId(int id)
    {
        if (await _mainSet.AnyAsync(io => io.ChanellId == id))
        {
            var res = await _mainSet.FirstOrDefaultAsync(io => io.ChanellId == id);
            return res;
        }
        Console.WriteLine("transkoderi araa gansazggbruli");
        return null;
    }
}
