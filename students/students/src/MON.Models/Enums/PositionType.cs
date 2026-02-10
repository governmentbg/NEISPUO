using System.ComponentModel;

namespace MON.Models.Enums
{
    /// <summary>
    /// Номенклатура за позиция на ученик. Таблица core.Position.
    /// </summary>
    public enum PositionType
    {
        /// <summary>
        /// персонал
        /// </summary>
        [Description("персонал")]
        Staff = 2,
        /// <summary>
        /// учащ (училище/ДГ)
        /// </summary>
        [Description("учащ (училище/ДГ)")]
        Student = 3,
        /// <summary>
        /// учащ (друга институция
        /// </summary>
        [Description("учащ (друга институция)")]
        StudentOtherInstitution = 7,
        /// <summary>
        /// учащ (ПЛР
        /// </summary>
        [Description("учащ (ПЛР)")]
        StudentPersDevelopmentSupport = 8,
        /// <summary>
        /// отписан
        /// </summary>
        [Description("отписан")]
        Discharged = 9,
        /// <summary>
        /// Unknown
        /// </summary>
        [Description("учащ (ЦСОП)")]
        StudentSpecialNeeds = 10,

        /// <summary>
        /// Unknown
        /// </summary>
        [Description("учащ (учащ ПО над 16г.)")]
        ProfessionalEducation = 11
    }
}
