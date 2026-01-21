const seedrandom = require('seedrandom');
seedrandom('seed_20210901', { global: true });

const fs = require('fs');
const path = require('path');
const _ = require('lodash');

const {
  randomMaleName,
  randomMaleFamilyName,
  randomFemaleName,
  randomFemaleFamilyName,
  randomDate,
  randomEGN
} = require('./random');

const baseDir = process.argv[2];
const files = process.argv.slice(3);

const transliterationMap = {
  а: 'a',
  б: 'b',
  в: 'v',
  г: 'g',
  д: 'd',
  е: 'e',
  ж: 'zh',
  з: 'z',
  и: 'i',
  й: 'y',
  к: 'k',
  л: 'l',
  м: 'm',
  н: 'n',
  о: 'o',
  п: 'p',
  р: 'r',
  с: 's',
  т: 't',
  у: 'u',
  ф: 'f',
  х: 'h',
  ц: 'ts',
  ч: 'ch',
  ш: 'sh',
  щ: 'sht',
  ъ: 'a',
  ь: 'y',
  ю: 'yu',
  я: 'ya'
};

const transliterate = (letter) => transliterationMap[letter.toLowerCase()];

const usedEGNs = {};

const getNames = (count, birthYear) =>
  [...new Array(count)]
    .map((__, i) => {
      const m = i % 2 === 0;
      const firstName = m ? randomMaleName() : randomFemaleName();
      const middleName = m ? randomMaleFamilyName() : randomFemaleFamilyName();
      const lastName = m ? randomMaleFamilyName() : randomFemaleFamilyName();
      const publicEduNumber =
        transliterate(firstName[0]) + transliterate(lastName[0]) + _.random(10000, 99999) + '@mon.bg';
      const { day, mon, year } = randomDate({ year: birthYear });
      const bdate = `${year}-${_.padStart(mon, 2, '0')}-${_.padStart(day, 2, '0')}`;

      let iter = 0;
      let egn;
      do {
        egn = randomEGN({ male: m, day, mon, year });
        iter++;
      } while (usedEGNs[egn] && iter < 3);

      if (iter > 3) {
        throw new Error('Failed to generate random egn');
      }

      usedEGNs[egn] = true;

      const s = [firstName, middleName, lastName, egn, publicEduNumber].map((c) => `'${c}'`).join(', ');
      return `select ${i}, ${s}, CONVERT(DATE, '${bdate}'), ${m ? 1 : 2}`;
    })
    .join(' UNION ALL\n');

const migrateFile = (filename) => {
  const lineCount = fs.readFileSync(filename, 'utf8').match(/^/gm).length;

  const extension = path.extname(filename);
  const basename = path.basename(filename);
  const tableName = basename.substring(0, basename.lastIndexOf(extension)) + 'Names';

  const header = `GO
PRINT 'Insert ${tableName}'
GO

create table [#${tableName}] (
    [Order]             INT             NOT NULL,
    [FirstName]         NVARCHAR (200)  NOT NULL,
    [MiddleName]        NVARCHAR (200)  NOT NULL,
    [LastName]          NVARCHAR (200)  NOT NULL,
    [EGN]               NVARCHAR (200)  NOT NULL,
    [PublicEduNumber]   NVARCHAR (200)  NOT NULL,
    [BirthDate]         DATE            NOT NULL,
    [Gender]            INT             NOT NULL,
)

-- randomly genereted names and egns
insert [#${tableName}] ([Order],[FirstName],[MiddleName],[LastName],[EGN],[PublicEduNumber],[BirthDate], [Gender])
`;

  const footer = `

GO
`;

  const isStudentsFile = basename.match(/students/i) != null;
  const birthYear = isStudentsFile ? 2010 : null;
  const namesFilename = path.join(path.dirname(filename), tableName + extension);
  const namesFileContent = header + getNames(lineCount, birthYear) + footer;

  fs.writeFileSync(namesFilename, namesFileContent, 'utf8');
};

files.forEach((file) => {
  const filename = path.resolve(baseDir, file);

  migrateFile(filename);
});
