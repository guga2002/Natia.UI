namespace Natia.Core.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task Add(T item);

    Task Remove(int id);

    Task View(int id);
}
