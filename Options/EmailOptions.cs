using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.Options
{
    public class EmailOptions
    {
        /// <summary>
        /// Smtp server
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// From email
        /// </summary>
        public string FromEmail { get; set; }
    }
}
