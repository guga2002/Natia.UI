using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories
{
    public class Emr60InfoRepository : BaseRepository<Emr60Info>, IEmr60InfoRepository
    {
        public Emr60InfoRepository(SpeakerDbContext context) : base(context)
        {
        }

        public async Task<int> GetEmrCodeByName(string Port)
        {
            var res = await _mainSet.FirstOrDefaultAsync(io => io.Port == Port);
            if (res == null)
                return 0;
            return res.SourceEmr;
        }

        public async Task<Emr60Info?> GetEmrInfoByCHanellName(string Port)
        {
            var res = await _mainSet.FirstOrDefaultAsync(io => io.Port == Port);
            if (res == null) return null;
            return res;
        }
    }
}
