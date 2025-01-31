using Natia.Core.Entities;

namespace Natia.Core.Interfaces
{
    public interface IInfoRepository : IBaseRepository<Infos>
    {
        Task<Infos> GetInfoByChanellId(int id);
    }
}
