using Microsoft.EntityFrameworkCore;
using Natia.Core.Context;

namespace Natia.Core.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly SpeakerDbContext _context;
        protected readonly DbSet<T> _mainSet;
        protected BaseRepository(SpeakerDbContext context)
        {
            _context = context;
            _mainSet = context.Set<T>();
        }
        public async Task Add(T item)
        {

            await _mainSet.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        public async Task Remove(int id)
        {
            var res = await _mainSet.FindAsync(id);
            if (res != null)
            {
                _mainSet.Remove(res);
                await _context.SaveChangesAsync();
            }
        }
        public async Task View(int id)
        {
            var res = await _mainSet.FindAsync(id);

            if (res is not null)
            {
                //MessageBox.Show(res.Name.ToString());
            }
            else
            {
                // MessageBox.Show("bazashi info ar aris");
            }
        }
    }
}
