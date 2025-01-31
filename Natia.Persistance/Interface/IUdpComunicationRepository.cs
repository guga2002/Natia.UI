using Natia.Persistance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natia.Persistance.Interface
{
    public interface IUdpComunicationRepository
    {
        Task<string> Receive();

        Task<List<ExcellDataMode3l>> StartTcpServer();
    }
}
