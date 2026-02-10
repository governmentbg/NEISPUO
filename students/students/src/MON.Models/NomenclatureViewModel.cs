using System;

namespace MON.Models
{
    public sealed class NomenclatureViewModel : DropdownViewModel
    {
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
