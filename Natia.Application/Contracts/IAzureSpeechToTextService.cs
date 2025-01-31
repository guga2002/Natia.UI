namespace Natia.Application.Contracts
{
    public interface IAzureSpeechToTextService
    {
        Task<byte[]> SpeakeNow(string text, string languageName = "ka-GE-EkaNeural");
        Task<string> DecodeFileName(string Name);
    }
}
