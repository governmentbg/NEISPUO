namespace MON.Models.Dynamic
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class DynamicEntities
    {
        [Required]
        [Description("Списък с описаните типове обекти за редакция.")]
        public List<DynamicEntity> Entities { get; set; }

        [Required]
        [Description("Глобални правила за сигурност. Прилагат се към всеки тип обект от свойството Entities.")]
        public DynamicEntitySecurity Security { get; set; }
    }
}
