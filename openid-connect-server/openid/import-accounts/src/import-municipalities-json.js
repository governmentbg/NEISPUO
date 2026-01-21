const fs = require("fs");
const path = require("path");

const IN_FILE_NAME = "exported_municipalities.json";
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "municipalites.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);


const municipalites = JSON.parse(fs.readFileSync(IN_FILE_FULL_PATH))
const dbName = '[neispuo-cp]'

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRANSACTION;
    SET IDENTITY_INSERT SysUser ON;


    DECLARE @lastCreatedId int; -- Variable to store the lastCreatedId.
    `
);
for (const ruo of municipalites) {
    const sql = `
    --
    -- ${ruo.FirstName} ${ruo.LastName}
    --
    INSERT INTO ${dbName}.core.Person (FirstName,LastName) VALUES
        ('${ruo.FirstName}','${ruo.LastName}');

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO ${dbName}.core.SysUser (SysUserID, Username, Password, IsAzureUser, PersonID) VALUES
        (${ruo.SysUserID}, '${ruo.Username}', NULL, 1, @lastCreatedId);

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO ${dbName}.core.SysUserSysRole (SysUserId,SysRoleId,MunicipalityID) VALUES
        (${ruo.SysUserID}, ${ruo.SysRoleID}, ${ruo.MunicipalityID });
    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, "\n SET IDENTITY_INSERT SysUser OFF; COMMIT\n");
