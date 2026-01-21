const fs = require("fs");
const path = require("path");

const IN_FILE_NAME = "categories.json";
const IN_FILE_FULL_PATH = path.resolve(__dirname, "..", "files", IN_FILE_NAME);

const OUT_FILE_NAME = "categories.sql";
const OUT_FILE_FULL_PATH = path.resolve(__dirname, "..", OUT_FILE_NAME);


const categories = JSON.parse(fs.readFileSync(IN_FILE_FULL_PATH))
const dbName = '[neispuo-cp]'

fs.writeFileSync(
    OUT_FILE_FULL_PATH,
    `BEGIN TRANSACTION;
        SET IDENTITY_INSERT ${dbName}.portal.Category ON;
    `
);
for (const category of categories) {
    const sql = `
    INSERT INTO ${dbName}.portal.Category (CategoryID, Name) VALUES
        (${category.CategoryID},'${category.Name}');


    `;
    fs.appendFileSync(OUT_FILE_FULL_PATH, sql);
}
fs.appendFileSync(OUT_FILE_FULL_PATH, `\nSET IDENTITY_INSERT ${dbName}.portal.Category OFF; COMMIT\n`);
