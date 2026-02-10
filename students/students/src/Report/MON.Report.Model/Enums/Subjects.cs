namespace MON.Report.Model.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public enum SubjectsEnum
    {
        [Description("Български език и литература")]
        BEL = 1,
        [Description("Математика")]
        Math = 2,
        [Description("География и икономика")]
        Geography = 13,
        [Description("История и цивилизации")]
        History = 18,
        [Description("Физика и астрономия")]
        Physics = 15,
        [Description("Човекът и природата")]
        HumanAndNature = 27,
        [Description("Биология и здравно образование")]
        Biology = 28,
        [Description("Химия и опазване на околната среда")]
        Chemistry = 29,
        [Description("Музика")]
        Music = 30,
        [Description("Изобразително изкуство")]
        Art = 31,
        [Description("Информационни технологии")]
        IT = 36,
        [Description("Физическо възпитание и спорт")]
        Sport = 45,
        [Description("Технологии и предприемачество")]
        Technology = 72,
        [Description("Теория и практика на професията")]
        TheoryAndPracticeOfProfession = 93
    }

    public static class Subjects
    {
        public const int BEL = 1;
        public const int Math = 2;
        public const int Geography = 13;
        public const int History = 18;
        public const int Physics = 15;
        public const int HumanAndNature = 27;
        public const int Biology = 28;
        public const int Chemistry = 29;
        public const int Music = 30;
        public const int Art = 31;
        public const int IT = 36;
        public const int Sport = 45;
        public const int Technology = 72;
    }
}
