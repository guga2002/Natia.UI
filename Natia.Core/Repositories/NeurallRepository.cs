using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Core.Repositories
{
    public class NeurallRepository : BaseRepository<Neurall>, INeuralRepository
    {
        public NeurallRepository(SpeakerDbContext context) : base(context)
        {
        }

        public async Task<bool> AddNewRecord(Neurall record)
        {
            if (await _mainSet.AnyAsync(io => io.WhatWasTopic == record.WhatWasTopic &&
            io.WhatNatiaSaid == record.WhatNatiaSaid))
            {
                return false;
            }
            await _mainSet.AddAsync(record);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteRecord(int id)
        {
            var record = await _mainSet.FindAsync(id);
            if (record is not null)
            {
                _mainSet.Remove(record);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Neurall>> GetAll()
        {
            return await _mainSet.ToListAsync();
        }

        public async Task<Neurall> GetRecordById(int id)
        {
            return await _mainSet.FirstOrDefaultAsync(io => io.Id == id);
        }

        public async Task<bool> RecordExist(string text)
        {
            return await _mainSet.AnyAsync(io => io.WhatNatiaSaid == text);
        }

        public async Task<bool> UpdateRecord(Neurall record)
        {
            _mainSet.Update(record);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
