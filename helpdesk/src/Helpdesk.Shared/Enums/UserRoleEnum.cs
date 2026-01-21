using System.ComponentModel;

namespace Helpdesk.Shared.Enums
{
    public enum UserRoleEnum
    {
        [Description("Институция")]
        School = 0, //
        [Description("МОН")]
        Mon = 1, //
        [Description("РУО")]
        Ruo = 2, //
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
        RuoExpert = 9, //
        [Description("ЦИОО")]
        CIOO = 10, // като Мон ескперт
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
        [Description("Консорциум поддръжка")]
        Consortium = 18,
        [Description("Счетоводител")]
        Accountant = 20

    }
}
