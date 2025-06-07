using Natia.Core.Entities;

namespace Natia.Core.Interfaces;

public interface IChanellRepository : IBaseRepository<Chanells>
{
    Task<Chanells?> GetChanellByPort(int port);

    Task<Chanells?> GetByID(int id);
}
