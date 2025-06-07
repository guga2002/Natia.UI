using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories
{
    public class ChanellRepository : BaseRepository<Chanells>, IChanellRepository
    {
        public ChanellRepository(SpeakerDbContext context) : base(context)
        {
        }

        public async Task<Chanells?> GetByID(int id)
        {
            return await _mainSet.Where(io => io.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Chanells?> GetChanellByPort(int port)
        {
            if (await _mainSet.AnyAsync(io => io.PortIn250 == port))
            {
                var res = await _mainSet.FirstOrDefaultAsync(io => io.PortIn250 == port);
                return res;
            }
            await Console.Out.WriteLineAsync("arxi ara  damatebuli");
            return null;
        }
    }
}
