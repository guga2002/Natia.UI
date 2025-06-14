using Natia.Application.Contracts;
using Natia.Persistance.Interface;

namespace Natia.Application.Services;

public class SounServices : ISoundService
{
    private readonly ISoundRepository soundRepository;
    private readonly IAzureSpeechToTextService _azureSpeechToTextService;

    public SounServices(ISoundRepository soundRepository, IAzureSpeechToTextService azureSpeechToTextService)
    {
        this.soundRepository = soundRepository;
        _azureSpeechToTextService = azureSpeechToTextService;
    }

    public async Task<byte[]?> SpeakNow(string text, int second = 1)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            return await soundRepository.SpeakNow(text, second);
        }
        catch (Exception)
        {
            return await soundRepository.SpeakNow(text, second);
        }
    }
}
