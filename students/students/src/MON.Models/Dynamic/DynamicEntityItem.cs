using System.ComponentModel.DataAnnotations;

namespace MON.Models.Dynamic
{
    public class DynamicEntityItem
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
        public string ColumnName { get; set; }
        /// <summary>
        /// <see cref="Enums.DynamicFieldTypeEnum"/>
        /// </summary>
        public string Type { get; set; }
        public string Label { get; set; }
        public string LabelEn { get; set; }
        public string Class { get; set; }
        public int Order { get; set; }
        public bool Readonly { get; set; }
        public bool Required { get; set; }
        /// <summary>
        /// Добавя бутон за изчисване на стойността на Input-a.
        /// </summary>
        public bool Clearable { get; set; }

        /// <summary>
        /// Определя дали ще се покаже като колона в грида.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Определя дали ще се филтрира в грида по колоната със стойноти от това поле.
        /// </summary>
        public bool Filterable { get; set; }

        /// <summary>
        /// Определя дали ще се покаже в динамичните форми.
        /// </summary>
        public bool Editable { get; set; } = true;


        /// <summary>
        /// Ограничени за минимална стойност при NumberField или минималната дължина при TextField.
        /// </summary>
        public decimal? Min { get; set; }
        /// <summary>
        /// Ограничени за максимална стойност при NumberField или максимална дължина при TextField.
        /// </summary>
        public decimal? Max { get; set; }

        /// <summary>
        /// При форматиране на датите https://www.mssqltips.com/sqlservertip/1145/date-and-time-conversions-using-sql-server/
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// Използва се от SelectList.
        /// </summary>
        public string OptionsUrl { get; set; }
        /// <summary>
        /// Използва се от SelectList.
        /// </summary>
        public bool DeferOptionsLoading { get; set; }
        /// <summary>
        /// Използва се от SelectList.
        /// </summary>
        public bool ShowDeferredLoadingHint { get; set; }
        /// <summary>
        /// Използва се от SelectList.
        /// </summary>
        public bool ReturnObject { get; set; }
        /// <summary>
        /// Използва се от SelectList. Има на свойството, което се използва за text на SelectList-а.
        /// </summary>
        public string ItemText { get; set; }
        /// <summary>
        /// Използва се от SelectList. Има на свойството, което се използва за value на SelectList-а.
        /// </summary>
        public string ItemValue { get; set; }


        public string Cols { get; set; } = "12";
        public string Xl { get; set; }
        public string Lg { get; set; }
        public string Md { get; set; }
        public string Sm { get; set; }

        public string Hint { get; set; }
        public bool? PersistentHint { get; set; }
        public string ContextInfo { get; set; }

        public bool? ShowGRAOSearch { get; set; }
        public object DefaultValue { get; set; }
    }
}
