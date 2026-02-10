using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MON.Models.Dynamic
{

    public class DynamicEntity
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        public string TitleEn { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Description("Име на схемата в БД.")]
        public string DbSchemaName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Description("Име на таблицата в БД.")]
        public string DbTableName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Description("Колона от таблицата в ДБ, по която ще се сортира по подразбиране. Използва се за визуализирането в грид.")]
        public string DefaultOrderBy { get; set; }

        [Description("Позволява създаеане на нов запис.")]
        public bool AllowCreate { get; set; }

        [Description("Позволява редактиране на запис.")]
        public bool AllowUpdate { get; set; }

        [Description("Позволява изтриване на запис.")]
        public bool AllowDelete { get; set; }


        public List<DynamicEntitySection> Sections { get; set; }

        [Required]
        [Description("Правила за сигурност. Допълват глобалните, ако има.")]
        public DynamicEntitySecurity Security { get; set; }
    }
}
