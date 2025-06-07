namespace Natia.Application.Contracts;

public interface ISoundService
{
    Task<byte[]?> SpeakNow(string text, int second = 1);
}
