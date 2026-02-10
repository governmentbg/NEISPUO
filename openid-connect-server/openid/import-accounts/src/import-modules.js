const fs = require("fs");
const path = require("path");

const IN_FILE_NAME = "modules.json";
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "modules.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);


const modules = JSON.parse(fs.readFileSync(IN_FILE_FULL_PATH))
const dbName = '[neispuo-cp]'

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRANSACTION;
        SET IDENTITY_INSERT ${dbName}.portal.Module ON;
    `
);
for (const module of modules) {
    const sql = `
    INSERT INTO ${dbName}.portal.Module (ModuleID, Name, CategoryID) VALUES
        (${module.ModuleID},'${module.Name}', ${module.CategoryID});


    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, `\nSET IDENTITY_INSERT ${dbName}.portal.Module OFF; COMMIT\n`);
