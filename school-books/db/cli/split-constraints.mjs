import fs from 'fs';
import path from 'path';

const baseDir = process.argv[2];
const files = process.argv.slice(3);

const migrateFile = (filename) => {
  const content = fs.readFileSync(filename, 'utf8');

  let mainFile = content;
  let constraintFile = null;

  const match = content.match(/^ALTER TABLE/m);

  if (match) {
    mainFile = content.substring(0, match.index);
    constraintFile = content.substr(match.index);
  }

  if (mainFile !== content) {
    fs.writeFileSync(filename, mainFile, 'utf8');

    const extension = path.extname(filename);
    const constraintFilename = path.join(
      path.dirname(filename),
      path.basename(filename).replace(extension, '.fks' + extension)
    );

    fs.writeFileSync(constraintFilename, constraintFile, 'utf8');
  }
};

files.forEach((file) => {
  const filename = path.resolve(baseDir, file);

  migrateFile(filename);
});
