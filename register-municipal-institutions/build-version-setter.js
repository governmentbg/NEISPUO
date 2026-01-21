const { execSync } = require('child_process');
const fs = require('fs');

const nextVersion = process.argv[2];
if (!nextVersion) {
  throw new Error('Empty version.');
}

const packageFiles = [
  './backend/package.json',
  './backend/package-lock.json',
  './frontend/package.json',
  './frontend/package-lock.json',
  './package.json',
  './package-lock.json',
];

for (const pf of packageFiles) {
  const packageJson = JSON.parse(fs.readFileSync(pf));
  packageJson.version = nextVersion;
  fs.writeFileSync(pf, JSON.stringify(packageJson));
  execSync(`node_modules/.bin/prettier ${pf} --write ${pf}`);
  execSync(`git add ${pf}`);
}

const filesChanged = execSync(`git status --porcelain`).toString();
if (!filesChanged) {
  console.log('No version update detected.');
  return;
}
execSync('git commit -m "build: [skip ci] [skip release] update version"');
const branchName = execSync('git rev-parse --abbrev-ref HEAD').toString();
execSync(
  `git remote set-url origin https://dev:${process.env.GL_TOKEN}@gitlab.com/dssbg/mon-neispuo-register-municipal-institutions.git`,
);
execSync(`git push -u origin ${branchName}`);
