namespace Helpdesk.Models.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AntiVirusConfig
    {
        public bool Enabled { get; set; }
        public int MaxSize { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
