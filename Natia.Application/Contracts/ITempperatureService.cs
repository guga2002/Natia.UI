namespace Natia.Application.Contracts
{
    public interface ITempperatureService
    {
        Task<(string, string)> GetCurrentData();
    }
}
