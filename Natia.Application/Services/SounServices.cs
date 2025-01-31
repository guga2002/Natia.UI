using Natia.Application.Contracts;
using Natia.Persistance.Interface;

namespace Natia.Application.Services
{
    public class SounServices : ISoundService
    {
        private readonly ISoundRepository soundRepository;

        public SounServices(ISoundRepository soundRepository)
        {
            this.soundRepository = soundRepository;
        }

        public async Task<byte[]> SpeakNow(string text, int second = 1)
        {
            return await soundRepository.SpeakNow(text, second);
        }
    }
}
