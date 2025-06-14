using Natia.Application.Dtos;
using Natia.Core.Entities;

namespace Natia.Application.Contracts;

public interface IChanellService : IcrudService<ChanellDto>
{
    Task<ChanellDto?> GetChanellByPort(int port);

    Task<Chanells?> GetByID(int id);
}
