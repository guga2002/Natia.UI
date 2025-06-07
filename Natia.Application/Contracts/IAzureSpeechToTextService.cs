namespace Natia.Application.Contracts;

public interface IAzureSpeechToTextService
{
    Task<byte[]> ConvertTextToSpeechAsync(string text, string language = "ka-GE", string voiceName = "ka-GE-EkaNeural");
}
