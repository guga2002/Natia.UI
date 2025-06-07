using Natia.Core.Entities;

namespace Natia.Core.Interfaces;

public interface IDesclamblerRepository : IBaseRepository<Desclamblers>
{
    Task<Desclamblers?> GetDesclamblerInfoById(int id);
}
