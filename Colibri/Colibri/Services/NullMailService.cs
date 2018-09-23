using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colibri.Services
{
    /*
     * Testing the MailService
     * 
     * do not send eMails, but write into the console
     */
    public class NullMailService : IMailService
    {
        // create a private Field for the Logger
        private readonly ILogger<NullMailService> _logger;

        // inject a Logger
        public NullMailService(ILogger<NullMailService> logger)
        {
            _logger = logger;
        }

        // Log the message. Test: dummy
        public void SendMessage(string to, string subject, string body)
        {
            _logger.LogInformation($"To: {to} Subject: {subject} Body: {body}");
        }
    }
}
