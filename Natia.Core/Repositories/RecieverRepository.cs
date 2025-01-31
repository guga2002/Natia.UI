using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories
{
    public class RecieverRepository : BaseRepository<Reciever>, IRecieverRepository
    {
        public RecieverRepository(SpeakerDbContext context) : base(context)
        {
        }

        public async Task<Chanells> GetChanellIdByCardandPort(int card, int port)
        {
            var res = await _mainSet.Include(io => io.Chanell).FirstOrDefaultAsync(io => io.Card == card && io.Port == port);
            if (res is not null)
            {
                return res?.Chanell;
            }
            return null;
        }

        public async Task<Reciever> GetRecieverInfoById(int id)
        {
            return await _mainSet.FirstOrDefaultAsync(io => io.ChanellId == id);
        }
    }
}
