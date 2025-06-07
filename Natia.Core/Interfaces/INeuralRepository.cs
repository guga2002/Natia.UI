using Natia.Core.Entities;

namespace Natia.Core.Interfaces;

public interface INeuralRepository
{
    Task<bool> AddNewRecord(Neurall record);

    Task<bool> DeleteRecord(int id);

    Task<bool> UpdateRecord(Neurall record);

    Task<Neurall?> GetRecordById(int id);

    Task<IEnumerable<Neurall>> GetAll();

    Task<bool> RecordExist(string text);
}
