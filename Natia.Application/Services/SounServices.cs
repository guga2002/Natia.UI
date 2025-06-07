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
            if (DateTime.Now.DayOfWeek  is DayOfWeek.Monday or DayOfWeek.Wednesday or DayOfWeek.Friday or DayOfWeek.Tuesday)
            {
                return await soundRepository.SpeakNow(text, second);
            }

            var hour = DateTime.Now.Hour;
            string voice = (hour >= 22 || hour < 6)
                ? "ka-GE-GiorgiNeural" 
                : "ka-GE-EkaNeural";

            var azureTry = await _azureSpeechToTextService.ConvertTextToSpeechAsync(text, "ka-GE", voice);
            if (azureTry != null && azureTry.Length > 0)
            {
                return azureTry;
            }

            throw new ArgumentException("Azure speech synthesis failed or returned no data.");
        }
        catch (Exception)
        {
            return await soundRepository.SpeakNow(text, second);
        }
    }
}
