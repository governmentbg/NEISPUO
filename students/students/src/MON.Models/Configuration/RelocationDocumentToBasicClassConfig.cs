namespace MON.Models.Configuration
{
    using System.Collections.Generic;

    public class RelocationDocumentToBasicClassConfig
    {
        public string DocumentPath { get; set; }
        public List<int> BasicClasses { get; set; }
    }
}
