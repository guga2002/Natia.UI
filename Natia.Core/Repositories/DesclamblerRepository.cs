using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories;

public class DesclamblerRepository : BaseRepository<Desclamblers>, IDesclamblerRepository
{
    public DesclamblerRepository(SpeakerDbContext context) : base(context)
    {
    }

    public async Task<Desclamblers?> GetDesclamblerInfoById(int id)
    {
        return await _mainSet.FirstOrDefaultAsync(io => io.ChanellId == id);
    }
}
