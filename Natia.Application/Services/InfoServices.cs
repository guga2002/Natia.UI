using Natia.Application.Contracts;
using Natia.Application.Dtos;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Application.Services
{
    public class InfoServices : IInfoService
    {
        private readonly IInfoRepository repos;

        public InfoServices(IInfoRepository rep)
        {
            repos = rep;
        }

        public async Task Add(InfoDto item)
        {
            await repos.Add(new Infos()
            {
                AlarmMessage = item.AlarmMessage,
                CHanellId = item.ChanellId,
            });
        }

        public async Task<InfoDto> GetInfoByChanellId(int id)
        {
            var res = await repos.GetInfoByChanellId(id);
            if (res is not null)
            {
                return new InfoDto()
                {
                    AlarmMessage = res.AlarmMessage,
                    ChanellId = res.CHanellId,
                };
            }
            return new InfoDto()
            {
                AlarmMessage = ""
            };
        }

        public async Task Remove(int id)
        {
            await repos.Remove(id);
        }

        public async Task View(int id)
        {
            await repos.View(id);
        }
    }
}
