using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories
{
    public class InfoRepository : BaseRepository<Infos>, IInfoRepository
    {
        public InfoRepository(SpeakerDbContext context) : base(context)
        {
        }

        public async Task<Infos> GetInfoByChanellId(int id)
        {

            if (await _mainSet.AnyAsync(io => io.CHanellId == id))
            {
                var res = await _mainSet.FirstOrDefaultAsync(io => io.CHanellId == id);

                return res;
            }
            await Console.Out.WriteLineAsync("info ara gansazgvruli");
            return null;
        }
    }
}
