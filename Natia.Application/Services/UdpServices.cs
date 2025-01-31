using Natia.Application.Contracts;
using Natia.Persistance.Interface;
using Natia.Persistance.Model;

namespace Natia.Application.Services
{
    public class UdpServices : IUdpService
    {
        private readonly IUdpComunicationRepository ser;
        public UdpServices(IUdpComunicationRepository ser)
        {
            this.ser = ser;
        }
        public async Task<string> Receive()
        {
            return await ser.Receive();
        }

        public async Task<List<ExcellDataMode3l>> StartTcpServer()
        {
            return await ser.StartTcpServer();
        }
    }
}
