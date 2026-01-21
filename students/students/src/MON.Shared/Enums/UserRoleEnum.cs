using System.ComponentModel;

namespace MON.Shared.Enums
{
    public enum UserRoleEnum
    {
        [Description("Институция")]
        School = 0, //
        [Description("МОН")]
        Mon = 1, //
        [Description("РУО")]
        Ruo = 2, // РУО - Експерт ИО/АИ
        [Description("Община")]
        Municipality = 3,
        [Description("Друга бюджетна институция")]
        FinancingInstitution = 4,
        [Description("Учител")]
        Teacher = 5, //
        [Description("Ученик")]
        Student = 6,
        [Description("Родител")]
        Parent = 7,
        [Description("РУО - Експерт")]
        RuoExpert = 9, // РУО - Експерт
        [Description("ЦИОО")]
        CIOO = 10, // като Мон експерт
        [Description("Експерт от външна институция")]
        ExternalExpert = 11,
        [Description("МОН - експерт")]
        MonExpert = 12, //
        [Description("Институция (техн. сътрудник)")]
        InstitutionAssociate = 14, // като School
        [Description("МОН – д. ОБГУМ")]
        MonOBGUM = 15, // Да влизат в ЛОД-а да правят дипломи и да пишат награди + read нещата на МОН
        [Description("МОН – д. ОБГУМ и д. Финанси")]
        MonOBGUM_Finance = 16, // Да влизат в ЛОД-а да правят дипломи и да пишат награди + read нещата на МОН
        [Description("МОН - д. ЧРАО")]
        MonHR = 17,
        [Description("Консорциум")]
        Consortium = 18,
        [Description("Счетоводител")]
        Accountant = 20,

        // За вътрешна употреба - не са роли, които се подават от OIDC
        [Description("Институция - училище")]
        SchoolDirector = 101, //
        [Description("Институция - детска градина")]
        KindergartenDirector = 102, //
        [Description("Институция - ЦПЛР")]
        CPLRDirector = 103, //
        [Description("Институция - ЦСОП")]
        CSOPDirector = 104, //
        [Description("Институция - СОЗ")]
        SOZDirector = 105, //
    }
}
