const fs = require("fs");
const path = require("path");

const IN_FILE_NAME = "neispuo_codes.json"; // file contents is json array of institutionIds, eg: [1,2,3]
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "schools.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);

const schoolIds = JSON.parse(fs.readFileSync(IN_FILE_FULL_PATH))
const dbName = '[neispuo-cp]'

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @lastCreatedId int; -- Variable to store the lastCreatedId.
    `
);
for (const schoolId of schoolIds) {
    const sql = `
    --
    -- ${schoolId}@edu.mon.bg
    --
    INSERT INTO ${dbName}.core.Person (FirstName,MiddleName,LastName) VALUES
        ('Няма','Няма','Няма');

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO ${dbName}.core.SysUser (Username,Password,IsAzureUser,PersonID) VALUES
        ('${schoolId}@edu.mon.bg',NULL,1,@lastCreatedId);

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO ${dbName}.core.SysUserSysRole (SysUserId,SysRoleId,InstitutionID) VALUES
        (@lastCreatedId,0,${schoolId});
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
