using eCommerce.Application.Services.Interfaces.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.infrastructure.Services
{
    // Fix class declaration and implement the generic interface
    public class SerilogAdapter<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;

        public SerilogAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception ex) => _logger.LogError(ex, message);

        public void LogInformation(string message) => _logger.LogInformation(message);

        public void LogWarning(string message) => _logger.LogWarning(message);
    }
}
