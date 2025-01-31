using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using Natia.Application.Contracts;

namespace Natia.Application.Services
{
    public class AzureSpeechToTextService : IAzureSpeechToTextService
    {
        private readonly string _subscriptionKey;
        private readonly string _region;

        public AzureSpeechToTextService()
        {
            _subscriptionKey = "48325d19738e4b579735c9a00e92ac09";
            _region = "eastus";
        }

        public async Task<string> DecodeFileName(string Name)
        {
            await Task.Delay(1);
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(Name);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public async Task<byte[]> SpeakeNow(string text, string languageName)
        {
            try
            {
                var speechConfig = SpeechConfig.FromSubscription(_subscriptionKey, _region);
                speechConfig.SpeechSynthesisVoiceName = languageName;
                var synthesizer = new Microsoft.CognitiveServices.Speech.SpeechSynthesizer(speechConfig);
                var result = await synthesizer.SpeakTextAsync(text);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    return result.AudioData;
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
