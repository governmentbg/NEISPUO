namespace MON.Models.Finance
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class GridHeaderModel
    {
        public string text { get; set; }
        public string value { get; set; }
        public bool filterable { get; set; }
        public bool sortable { get; set; }
        public bool groupable { get; set; }
    }
}
