using Natia.Persistance.Interface;
using Natia.Persistance.Model;
using System.Speech.Synthesis;

namespace Natia.Persistance.Repositories;

public class SoundRepository : ISoundRepository
{
    private readonly NatiaSettings natiaSettings;
    private static Random _random= new();
    public SoundRepository()
    {
        natiaSettings = new NatiaSettings();
    }

    public async Task<byte[]> SpeakNow(string text, int baseRate = 2)
    {
        try
        {
            await Task.Delay(1);

            using (var synthesizer = new SpeechSynthesizer())
            using (var memoryStream = new MemoryStream())
            {
                var voices = synthesizer.GetInstalledVoices()
                                         .Where(voice => voice.Enabled)
                                         .ToList();

                var selectedVoice = voices.FirstOrDefault(voice =>
                    voice.VoiceInfo.Culture.Name.StartsWith(natiaSettings.Language) &&
                    voice.VoiceInfo.Name.Contains(natiaSettings.Model));

                if (selectedVoice != null)
                {
                    synthesizer.SelectVoice(selectedVoice.VoiceInfo.Name);

                    synthesizer.Rate = baseRate + _random.Next(-2, 2);
                    synthesizer.Volume = _random.Next(75, 101);

                    synthesizer.SetOutputToWaveStream(memoryStream);

                    synthesizer.Speak(text);

                    return memoryStream.ToArray();
                }
                else
                {
                    Console.WriteLine("No matching voice found.");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while speaking: {ex.Message}");
            return null;
        }
    }

}
