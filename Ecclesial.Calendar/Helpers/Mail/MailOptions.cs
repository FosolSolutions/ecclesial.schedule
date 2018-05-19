using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.Helpers.Mail
{
    public class MailOptions
    {
        public string AccountAddress { get; set; }
        public string AccountPassword { get; set; }
        public string SmtpDNS { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
        public int Timeout { get; set; } = 15000;
    }
}
