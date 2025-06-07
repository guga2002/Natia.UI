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
        var url = $"https://{_region}.tts.speech.microsoft.com/cognitiveservices/v1";
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        _httpClient.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm"); 

        var requestBody = $@"
        <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='{language}'>
            <voice name='{voiceName}'>{System.Net.WebUtility.HtmlEncode(text)}</voice>
        </speak>";

        var content = new StringContent(requestBody, Encoding.UTF8, "application/ssml+xml");
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

}
