namespace MON.Shared.Enums
{
    using System.ComponentModel;

    public enum RegBookTypeEnum
    {
        [Description("Регистрационна книга за издадените документи")]
        RegBookQualification = 1,
        [Description("Регистрационна книга за издадените дубликати на документи")]
        RegBookQualificationDuplicate = 2,
        [Description("Регистрационна книга за издадените удостоверения")]
        RegBookCertificate = 3,
        [Description("Регистрационна книга за издадените дубликати на удостоверения")]
        RegBookCertificateDuplicate = 4
    }
}
