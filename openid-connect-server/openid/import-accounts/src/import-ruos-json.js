const fs = require("fs");
const path = require("path");

const IN_FILE_NAME = "exported_ruos.json";
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "ruo.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);


const ruos = JSON.parse(fs.readFileSync(IN_FILE_FULL_PATH))
const dbName = '[neispuo-cp]'

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRANSACTION;
    SET IDENTITY_INSERT SysUser ON;


    DECLARE @lastCreatedId int; -- Variable to store the lastCreatedId.
    `
);
for (const ruo of ruos) {
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

    INSERT INTO ${dbName}.core.SysUserSysRole (SysUserId,SysRoleId,RegionID) VALUES
        (${ruo.SysUserID}, ${ruo.SysRoleID}, ${ruo.RegionID });
    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, "\n SET IDENTITY_INSERT SysUser OFF; COMMIT\n");
