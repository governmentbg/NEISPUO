using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models.Dynamic
{
    public class DynamicEntitySection
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public bool Visible { get; set; }
        public int Order { get; set; }
        public string Section { get; set; }
        public List<DynamicEntityItem> Items { get; set; }
    }
}
