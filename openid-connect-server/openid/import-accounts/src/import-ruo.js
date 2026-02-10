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
    `BEGIN TRANSACTION;

    DECLARE @lastCreatedId int; -- Variable to store the lastCreatedId.
    `
);
for (const ruo of ruos) {
    const sql = `
    --
    -- ${ruo.FName} ${ruo.MName} ${ruo.LName}
    --
    INSERT INTO neispuo.core.Person (FirstName,MiddleName,LastName) VALUES
        ('${ruo.FName}','${ruo.MName}','${ruo.LName}');

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO neispuo.core.SysUser (Username,Password,IsAzureUser,PersonID) VALUES
        ('${ruo.UserName}',NULL,1,@lastCreatedId);

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO neispuo.core.SysUserSysRole (SysUserId,SysRoleId,RegionID) VALUES
        (@lastCreatedId,2,${ruo.RegionId});
    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, "\nCOMMIT\n");
