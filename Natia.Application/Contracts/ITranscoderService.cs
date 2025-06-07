using Natia.Application.Dtos;

namespace Natia.Application.Contracts;

public interface ITranscoderService : IcrudService<TranscoderDto>
{
    Task<TranscoderDto?> GetTranscoderInfoByChanellId(int id);

    Task<int> GetChanellIdBycardandport(int card, int port);
}
