using Natia.Core.Entities;

namespace Natia.Core.Interfaces
{
    public interface IRecieverRepository : IBaseRepository<Reciever>
    {
        Task<Reciever> GetRecieverInfoById(int id);
        Task<Chanells> GetChanellIdByCardandPort(int card, int port);
    }
}
