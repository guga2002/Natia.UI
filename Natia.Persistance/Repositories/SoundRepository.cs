using Natia.Persistance.Interface;
using Natia.Persistance.Model;
using System.Speech.Synthesis;
using Microsoft.Extensions.Logging;

namespace Natia.Persistance.Repositories;

public class SoundRepository : ISoundRepository
{
    private readonly NatiaSettings natiaSettings;
    private static readonly Random _random = new();
    private readonly ILogger<SoundRepository> _logger;

    public SoundRepository(ILogger<SoundRepository> logger)
    {
        _logger = logger;
        natiaSettings = new NatiaSettings();
    }

    public async Task<byte[]> SpeakNow(string text, int baseRate = 2)
    {
        try
        {
            _logger.LogInformation("Starting speech synthesis for text: \"{Text}\" with baseRate: {BaseRate}", text, baseRate);

            await Task.Delay(1);

            using (var synthesizer = new SpeechSynthesizer())
            using (var memoryStream = new MemoryStream())
            {
                var voices = synthesizer.GetInstalledVoices()
                                         .Where(voice => voice.Enabled)
                                         .ToList();

                _logger.LogDebug("Found {VoiceCount} installed voices. Searching for language {Language} and model {Model}.",
                    voices.Count, natiaSettings.Language, natiaSettings.Model);

                var selectedVoice = voices.FirstOrDefault(voice =>
                    voice.VoiceInfo.Culture.Name.StartsWith(natiaSettings.Language) &&
                    voice.VoiceInfo.Name.Contains(natiaSettings.Model));

                if (selectedVoice != null)
                {
                    synthesizer.SelectVoice(selectedVoice.VoiceInfo.Name);

                    synthesizer.Rate = baseRate + _random.Next(-2, 2);
                    synthesizer.Volume = _random.Next(75, 101);

                    _logger.LogInformation("Selected voice: {VoiceName} | Rate: {Rate} | Volume: {Volume}",
                        selectedVoice.VoiceInfo.Name, synthesizer.Rate, synthesizer.Volume);

                    synthesizer.SetOutputToWaveStream(memoryStream);
                    synthesizer.Speak(text);

                    _logger.LogInformation("Speech synthesis completed successfully for text: \"{Text}\".", text);
                    return memoryStream.ToArray();
                }
                else
                {
                    _logger.LogWarning("No matching voice found for language {Language} and model {Model}.",
                        natiaSettings.Language, natiaSettings.Model);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating speech for text: \"{Text}\".", text);
            return null;
        }
    }
}
