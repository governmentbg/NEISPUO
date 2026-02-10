const fs = require("fs");
const path = require("path");

const IN_FILE_NAME = "exported_admins.json";
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "admins.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);


const monAdmins = JSON.parse(fs.readFileSync(IN_FILE_FULL_PATH))
const dbName = '[neispuo-cp]'

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRANSACTION;
    SET IDENTITY_INSERT SysUser ON;


    DECLARE @lastCreatedId int; -- Variable to store the lastCreatedId.
    `
);
for (const monAdmin of monAdmins) {
    const sql = `
    --
    -- ${monAdmin.FirstName} ${monAdmin.LastName}
    --
    INSERT INTO ${dbName}.core.Person (FirstName,LastName) VALUES
        ('${monAdmin.FirstName}','${monAdmin.LastName}');

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO ${dbName}.core.SysUser (SysUserID, Username, Password, IsAzureUser, PersonID) VALUES
        (${monAdmin.SysUserID}, '${monAdmin.Username}', NULL, 1, @lastCreatedId);

    SET @lastCreatedId = SCOPE_IDENTITY();

    INSERT INTO ${dbName}.core.SysUserSysRole (SysUserId,SysRoleId) VALUES
        (${monAdmin.SysUserID}, ${monAdmin.SysRoleID});
    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, "\n SET IDENTITY_INSERT SysUser OFF; COMMIT\n");
