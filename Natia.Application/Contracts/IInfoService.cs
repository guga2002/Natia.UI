using Natia.Application.Dtos;

namespace Natia.Application.Contracts;

public interface IInfoService : IcrudService<InfoDto>
{
    Task<InfoDto?> GetInfoByChanellId(int id);
}
