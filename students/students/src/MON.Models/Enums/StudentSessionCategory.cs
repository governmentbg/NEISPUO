namespace MON.Models.Enums
{
    using System.ComponentModel;
    /// <summary>
    /// Използва се за филтриране на записите от таблицата student.GradeCategory
    /// </summary>
    public enum StudentSessionCategory
    {
        ///<summary>Категорията в базата е с Description "Годишна" и я визуализираме като "Редовна"</summary>
        [Description("Редовна")]
        Final = 3,
    }
}
