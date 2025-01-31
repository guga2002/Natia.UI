using Natia.Persistance.Model;

namespace Natia.Application.Contracts
{
    public interface IUdpService
    {
        Task<string> Receive();
        Task<List<ExcellDataMode3l>> StartTcpServer();
    }
}
