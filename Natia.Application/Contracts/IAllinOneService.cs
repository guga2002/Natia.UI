using Natia.Core.Entities;

namespace Natia.Application.Contracts;

public interface IAllinOneService
{
    Task<Emr60Info?> GetInfoByChanellName(string Name);

    Task<Desclamblers?> GetDesclamblerInfoByChanellId(int id);

    Task<Reciever?> GetRecieverInfoByChanellId(int id);

    Task<string> GetPort(string Name);
}
