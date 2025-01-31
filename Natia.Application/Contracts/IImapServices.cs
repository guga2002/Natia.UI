using Natia.Application.Dtos;

namespace Natia.Application.Contracts
{
    public interface IImapServices
    {
        Task<List<MaillMessageDto>> CheckForNewMessage();
        Task<List<MaillMessageDto>> CheckforReplay();
    }
}
