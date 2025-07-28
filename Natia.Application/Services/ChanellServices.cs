using Microsoft.Extensions.Logging;
using Natia.Application.Contracts;
using Natia.Application.Dtos;
using Natia.Core.Entities;
using Natia.Core.Interfaces;

namespace Natia.Application.Services
{
    public class ChanellServices : IChanellService
    {
        private readonly IChanellRepository chan;
        private readonly ILogger<ChanellServices> _logger;

        public ChanellServices(IChanellRepository chan, ILogger<ChanellServices> logger)
        {
            this.chan = chan;
            _logger = logger;
        }

        public async Task Add(ChanellDto item)
        {
            try
            {
                await chan.Add(new Chanells()
                {
                    FromOptic = item.FromOptic,
                    ChanellFormat = item.ChanellFormat,
                    Name = item.Name,
                    PortIn250 = item.PortIn250,
                    NameForSPeak = item.NameForSpeake
                });
                _logger.LogInformation("Channel added: Name={Name}, Port={Port}", item.Name, item.PortIn250);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding channel: Name={Name}, Port={Port}", item.Name, item.PortIn250);
                throw;
            }
        }

        public Task<Chanells?> GetByID(int id)
        {
            _logger.LogInformation("Fetching channel by ID={ID}", id);
            return chan.GetByID(id);
        }

        public async Task<ChanellDto?> GetChanellByPort(int port)
        {
            try
            {
                _logger.LogInformation("Fetching channel by Port={Port}", port);
                var res = await chan.GetChanellByPort(port);
                if (res != null)
                {
                    _logger.LogInformation("Channel found: Name={Name}, Port={Port}", res.Name, res.PortIn250);
                    return new ChanellDto()
                    {
                        Id = res.Id,
                        ChanellFormat = res.ChanellFormat,
                        FromOptic = res.FromOptic,
                        Name = res.Name,
                        PortIn250 = res.PortIn250,
                        NameForSpeake = res.NameForSPeak,
                    };
                }

                _logger.LogWarning("No channel found for Port={Port}", port);
                return new ChanellDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching channel by Port={Port}", port);
                throw;
            }
        }

        public async Task Remove(int id)
        {
            try
            {
                await chan.Remove(id);
                _logger.LogInformation("Removed channel with ID={ID}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing channel with ID={ID}", id);
                throw;
            }
        }

        public async Task View(int id)
        {
            try
            {
                await chan.View(id);
                _logger.LogInformation("Viewed channel with ID={ID}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing channel with ID={ID}", id);
                throw;
            }
        }
    }
}
