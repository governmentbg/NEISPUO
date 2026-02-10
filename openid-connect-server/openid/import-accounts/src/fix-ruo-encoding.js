const fs = require("fs");
const path = require("path");
const XLSX = require("xlsx");

const IN_FILE_NAME = "Office 365 aкаунти РУО.xlsx";
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "ruo.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);

const workbook = XLSX.readFile(IN_FILE_FULL_PATH);
const jsonSheet = XLSX.utils.sheet_to_json(workbook.Sheets["Sheet1"]);

const ruos = jsonSheet
    .map((v, i) => {
        const RegionId = (String(v["Код област"]) || "").trim();
        const FName = (v["Име"] || "").trim();
        const MName = (v["Презиме"] || "").trim();
        const LName = (v["Фамилия"] || "Няма").trim();
        const UserName = (v["Username"] || "").trim();

        if (!RegionId || !FName || !LName || !UserName) {
            console.log(`
Problem found at row ${i + 2}
    RegionId: ${RegionId}
    FName: ${FName}
    MName: ${MName}
    LName: ${LName}
    UserName: ${UserName}
`);
            return null;
        }
        return { RegionId, FName, MName, LName, UserName };
    })
    .filter((v) => !!v);

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRY
    BEGIN TRANSACTION;
    `
);
for (const ruo of ruos) {
    const sql = `
    --
    -- ${ruo.UserName}: ${ruo.FName} ${ruo.MName} ${ruo.LName}
    --
    UPDATE neispuo.core.Person SET FirstName='${ruo.FName}',MiddleName='${ruo.MName}',LastName='${ruo.LName}' WHERE PersonID = (
        SELECT PersonID from neispuo.core.SysUser where Username='${ruo.UserName}'
    );
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
