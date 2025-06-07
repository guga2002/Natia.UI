using Natia.Core.Entities;

namespace Natia.Core.Interfaces;

public interface IEmr60InfoRepository
{
    Task<int> GetEmrCodeByName(string Port);

    Task<Emr60Info?> GetEmrInfoByCHanellName(string Port);
}
