using Natia.Core.Entities;

namespace Natia.Core.Interfaces
{
    public interface ITranscoderRepository : IBaseRepository<Transcoder>
    {
        Task<Transcoder> GetTranscoderInfoByCHanellId(int id);
        Task<int> GetChanellIdByCardandPort(int card, int port);
    }
}
