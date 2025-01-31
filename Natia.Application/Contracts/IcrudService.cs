namespace Natia.Application.Contracts
{
    public interface IcrudService<T> where T : class
    {
        Task Add(T item);
        Task Remove(int id);
        Task View(int id);
    }
}
