const fs = require("fs");
const path = require("path");

const OUT_FILE_NAME = "create-acc-person-sysusersysrole.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);

const accounts = [
    {
        UserName: "test-school-1@edu.mon.bg",
        FName: "Училище1",
        MName: "Училище1",
        LName: "Училище1",
        SysRoleID: 0,
        InstitutionID: 100004,
        BudgetingInstitutionID: null,
        MunicipalityID: null,
        RegionID: null,
    },
    {
        UserName: "test-school-2@edu.mon.bg",
        FName: "Училище2",
        MName: "Училище2",
        LName: "Училище2",
        SysRoleID: 0,
        InstitutionID: 100190,
        BudgetingInstitutionID: null,
        MunicipalityID: null,
        RegionID: null,
    },
    // { // created manually as not in sysusersysrole, but educationalState
    //     UserName: "test-teacher@edu.mon.bg",
    //     FName: "Учител",
    //     MName: "Учител",
    //     LName: "Учител",
    //     SysRoleID: 5,
    //     InstitutionID: null,
    //     BudgetingInstitutionID: null,
    //     MunicipalityID: null,
    //     RegionID: null,
    // },
    {
        UserName: "test-ruo-1@edu.mon.bg",
        FName: "РУО1",
        MName: "РУО1",
        LName: "РУО1",
        SysRoleID: 2,
        InstitutionID: null,
        BudgetingInstitutionID: null,
        MunicipalityID: null,
        RegionID: 22,
    },
    {
        UserName: "test-ruo-2@edu.mon.bg",
        FName: "РУО2",
        MName: "РУО2",
        LName: "РУО2",
        SysRoleID: 2,
        InstitutionID: null,
        BudgetingInstitutionID: null,
        MunicipalityID: null,
        RegionID: 16,
    },
    {
        UserName: "test-municipality-1@edu.mon.bg",
        FName: "Община1",
        MName: "Община1",
        LName: "Община1",
        SysRoleID: 3,
        InstitutionID: null,
        BudgetingInstitutionID: null,
        MunicipalityID: 155,
        RegionID: null,
    },
    {
        UserName: "test-municipality-2@edu.mon.bg",
        FName: "Община2",
        MName: "Община2",
        LName: "Община2",
        SysRoleID: 3,
        InstitutionID: null,
        BudgetingInstitutionID: null,
        MunicipalityID: 220,
        RegionID: null,
    },
    {
        UserName: "test-monadmin@edu.mon.bg",
        FName: "МОН",
        MName: "МОН",
        LName: "МОН",
        SysRoleID: 1,
        InstitutionID: null,
        BudgetingInstitutionID: null,
        MunicipalityID: null,
        RegionID: null,
    },
];

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @lastCreatedId int; -- Variable to store the lastCreatedId.
    `
);
for (const acc of accounts) {
    const sql = `
    --
    -- ${acc.UserName} ${acc.FName}
    --
    INSERT INTO neispuo.core.Person (FirstName,MiddleName,LastName) VALUES
        ('${acc.FName}','${acc.MName}','${acc.LName}');

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO neispuo.core.SysUser (Username,Password,IsAzureUser,PersonID) VALUES
        ('${acc.UserName}',NULL,1,@lastCreatedId);

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO neispuo.core.SysUserSysRole (SysUserID,SysRoleID,InstitutionID,BudgetingInstitutionID,MunicipalityID,RegionID) VALUES
        (@lastCreatedId,${acc.SysRoleID},${acc.InstitutionID},${acc.BudgetingInstitutionID},${acc.MunicipalityID},${acc.RegionID});
    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, `
    COMMIT
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()

    -- Use RAISERROR inside the CATCH block to return error
    -- information about the original error that caused
    -- execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH`);
