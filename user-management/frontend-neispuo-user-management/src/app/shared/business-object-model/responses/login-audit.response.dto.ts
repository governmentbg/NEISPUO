export class LoginAuditResponseDTO {
    LoginAuditID?: number;

    SysUserID?: number;

    Username?: string;

    SysRoleID?: number;

    SysRoleName?: string;

    InstitutionID?: number;

    InstitutionName?: string;

    RegionID?: number;

    RegionName?: string;

    MunicipalityID?: number;

    MunicipalityName?: string;

    BudgetingInstitutionID?: number;

    BudgetingInstitutionName?: string;

    PositionID?: number;

    IPSource?: string;

    CreatedOn?: Date; // (Automatically set on insert)
}
