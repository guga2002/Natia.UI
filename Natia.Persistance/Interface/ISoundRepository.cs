namespace Natia.Persistance.Interface;

public interface ISoundRepository
{
    Task<byte[]> SpeakNow(string text, int baseRate = 2);
}
