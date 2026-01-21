import fs from 'fs';
import path from 'path';
import { Connection, Request, TYPES } from 'tedious';
import { format, addMinutes } from 'date-fns';

const args = process.argv.slice(2);

const config = {
  server: args[0],
  authentication: {
    type: 'default',
    options: {
      userName: args[2],
      password: args[3]
    }
  },
  options: {
    database: args[4],
    port: parseInt(args[1], 10),
    encrypt: false,
    rowCollectionOnRequestCompletion: true,
    requestTimeout: 180000
  }
};

const scriptInstitutions = args[5]
  .replace(/^'|'$/g, '')
  .split(',')
  .map((id) => id.trim());
const scriptSchoolYears = args[6]
  .replace(/^'|'$/g, '')
  .split(',')
  .map((year) => year.trim());

const filepath = args[7];
const content = fs.readFileSync(filepath, 'utf8');

const connection = new Connection(config);

connection.on('connect', async function (err) {
  if (err) {
    console.log(err);
  }

  let match = null;
  const regex = /-- File:([^\s]+) Table:([^\s]+)( identity)?\n((.(?!-- File:))+)/gis;
  while ((match = regex.exec(content)) !== null) {
    const file = path.join(path.dirname(filepath), match[1]);
    const tableName = match[2];
    const identity = !!match[3];
    const sqlWithHeader = match[4];

    const data = await GenerateTableInserts(tableName, identity, sqlWithHeader);

    fs.writeFileSync(file, data);
  }

  process.exit(0);
});

connection.connect();

function GenerateTableInserts(tableName, identity, sqlWithHeader) {
  const lines = sqlWithHeader.split(/\n/);

  // lines from the beginning starting with comment (--) are consider additional file header, the rest are the sql
  // find the end of the header
  let headerEnd = 0;
  while (headerEnd < lines.length && lines[headerEnd].startsWith('--')) {
    headerEnd++;
  }

  const additionalHeader = lines
    .slice(0, headerEnd)
    .map((l) => l.substr(2))
    .join('\n');

  const sql = lines
    .slice(headerEnd)
    .join('\n')
    .replace(/\$\(ScriptInstitutions\)/g, scriptInstitutions)
    .replace(/\$\(ScriptSchoolYears\)/g, scriptSchoolYears);

  return new Promise((resolve, reject) => {
    const header = `GO
PRINT 'Insert ${tableName}'
GO

${additionalHeader ? additionalHeader + '\n\n' : ''}`;

    const footer = `

GO
`;

    const identity_on = identity
      ? `set identity_insert ${tableName} on;

`
      : '';

    const identity_off = identity
      ? `

set identity_insert ${tableName} off;`
      : '';

    let insertHeader = null;

    const request = new Request(sql, (err, rowCount, rows) => {
      if (err) {
        console.log(`Error executing SQL:\n${sql}`);
        reject(err);
      } else {
        let result = [];
        let currentIndex = 0;

        for (let columns of rows) {
          result.push(getDataRow(columns, rowCount, currentIndex, insertHeader));
          currentIndex++;
        }

        if (result.length) {
          resolve(header + identity_on + insertHeader + '\n' + result.join('\n') + identity_off + footer);
        } else {
          resolve(header + footer);
        }
      }
    });

    request.on('columnMetadata', (columns) => {
      insertHeader = `insert ${tableName} (${columns.map((c) => `[${c.colName}]`).join(',')})`;
    });

    connection.execSql(request);
  });
}

function getDataRow(columns, rowCount, currentIndex, insertHeader) {
  const rowData = [];

  for (let column of columns) {
    if (column.value == null) {
      rowData.push('NULL');
      continue;
    }

    switch (column.metadata.type.name) {
      case TYPES.VarChar.name:
      case TYPES.Char.name:
      case TYPES.Text.name:
      case TYPES.Xml.name:
        rowData.push(`'${escapeQuotes(column.value)}'`);
        break;
      case TYPES.NVarChar.name:
      case TYPES.NText.name:
      case TYPES.NChar.name:
        rowData.push(`'${escapeQuotes(column.value)}'`);
        break;
      case TYPES.Date.name:
      case TYPES.DateTime.name:
      case 'DateTimeN':
      case TYPES.DateTime2.name:
      case TYPES.SmallDateTime.name:
      case 'DateN':
      case 'DateTimeOffsetN':
      case 'DateTime2N':
        rowData.push(`'${formatDate(column.value)}'`);
        break;
      case TYPES.Time.name:
      case 'TimeN':
        rowData.push(`'${formatTime(column.value)}'`);
        break;
      case TYPES.Decimal.name:
      case TYPES.Numeric.name:
      case TYPES.Real.name:
      case TYPES.Float.name:
      case 'FloatN':
      case TYPES.Money.name:
      case TYPES.SmallMoney.name:
      case 'MoneyN':
      case 'DecimalN':
      case 'NumericN':
        // some collations use a comma for decimal places vs a period
        rowData.push(column.value.toString());
        break;
      case TYPES.UniqueIdentifier.name:
      case 'UniqueIdentifierN':
        rowData.push(`'{${column.value}}'`);
        break;
      case TYPES.Bit.name:
      case 'BitN':
        rowData.push(column.value ? '1' : '0');
        break;
      case TYPES.Int.name:
      case 'IntN':
      case TYPES.BigInt.name:
      case TYPES.SmallInt.name:
      case TYPES.TinyInt.name:
      case TYPES.Binary.name:
      case TYPES.Image.name:
      case TYPES.VarBinary.name:
        rowData.push(column.value);
        break;
      default:
        rowData.push(`'${column.value}'`);
        break;
    }
  }

  let tail = ' UNION ALL';

  if (currentIndex + 1 === rowCount) {
    tail = ';';
  } else if ((currentIndex + 1) % 5000 === 0) {
    // Performance issues can arise if there is an extremely large number of rows.
    // Insert a GO statement every so often.
    tail = `; \nGO\n${insertHeader}`;
  }

  // return select value1, value2, value...;
  return 'select ' + rowData.join(',') + tail;
}

function escapeQuotes(input) {
  return input.replace(/'+/g, "''");
}

function formatDate(date) {
  return format(addMinutes(date, date.getTimezoneOffset()), 'yyyy-MM-dd HH:mm:ss.SSS', {});
}

function formatTime(date) {
  return format(addMinutes(date, date.getTimezoneOffset()), 'HH:mm:ss', {});
}
