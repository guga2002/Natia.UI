using System.Text;
using Natia.Application.Contracts;

namespace Natia.Application.Services;

public class AzureSpeechToTextService : IAzureSpeechToTextService
{
    private readonly string _subscriptionKey;
    private readonly string _region;
    private readonly HttpClient _httpClient;

    public AzureSpeechToTextService(HttpClient httpClient)
    {
        _subscriptionKey = "7HQ1HuS4dsFB2sZwgF6ag1VeqcEFpi6INL9ZumoyNwKhGdyz966TJQQJ99BCAC5RqLJXJ3w3AAAAACOGG9YK";
        _region = "westeurope";
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<byte[]> ConvertTextToSpeechAsync(string text, string language = "ka-GE", string voiceName = "ka-GE-EkaNeural")
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty.", nameof(text));

        var url = $"https://{_region}.tts.speech.microsoft.com/cognitiveservices/v1";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        request.Headers.Add("User-Agent", "NatiaVoiceApp");
        request.Headers.Add("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm"); // WAV format

        var ssml = $@"
<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='{language}'>
  <voice name='{voiceName}'>{System.Net.WebUtility.HtmlEncode(text)}</voice>
</speak>";

        request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Azure TTS failed ({response.StatusCode}): {error}");
        }

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        if (mediaType == null || !mediaType.StartsWith("audio"))
        {
            var nonAudio = await response.Content.ReadAsStringAsync();
            throw new Exception($"Expected audio response, got: {nonAudio}");
        }

        var audioBytes = await response.Content.ReadAsByteArrayAsync();

        // Optional: Verify RIFF header to ensure it's a WAV file
        if (audioBytes.Length < 4 || Encoding.ASCII.GetString(audioBytes, 0, 4) != "RIFF")
        {
            throw new Exception("Returned audio is invalid or not in WAV format.");
        }

        return audioBytes;
    }



}
