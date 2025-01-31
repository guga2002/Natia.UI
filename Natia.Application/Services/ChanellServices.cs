using Natia.Application.Contracts;
using Natia.Application.Dtos;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Application.Services
{
    public class ChanellServices : IChanellService
    {
        private readonly IChanellRepository chan;

        public ChanellServices(IChanellRepository chan)
        {
            this.chan = chan;
        }

        public async Task Add(ChanellDto item)
        {
            await chan.Add(new Chanells()
            {
                FromOptic = item.FromOptic,
                ChanellFormat = item.ChanellFormat,
                Name = item.Name,
                PortIn250 = item.PortIn250,
                NameForSPeak = item.NameForSpeake
            });
        }

        public Task<Chanells> GetByID(int id)
        {
            return chan.GetByID(id);
        }

        public async Task<ChanellDto> GetChanellByPort(int port)
        {
            var res = await chan.GetChanellByPort(port);
            if (res != null)
            {
                return new ChanellDto()
                {
                    Id = res.Id,
                    ChanellFormat = res.ChanellFormat,
                    FromOptic = res.FromOptic,
                    Name = res.Name,
                    PortIn250 = res.PortIn250,
                    NameForSpeake = res.NameForSPeak,
                };
            }
            return new ChanellDto();
        }

        public async Task Remove(int id)
        {
            await chan.Remove(id);
        }

        public async Task View(int id)
        {
            await chan.View(id);
        }
    }
}
