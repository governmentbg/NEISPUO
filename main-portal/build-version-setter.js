const { execSync } = require('child_process');
const fs = require('fs');

const nextVersion = process.argv[2];
if (!nextVersion) {
  throw new Error('Empty version.');
}

const packageFiles = [
  './package.json',
  './package-lock.json',
  './backend-neispuo-main-portal/package.json',
  './backend-neispuo-main-portal/package-lock.json',
  './frontend-neispuo-main-portal/package.json',
  './frontend-neispuo-main-portal/package-lock.json',
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
const branchName = execSync('git rev-parse --abbrev-ref HEAD').toString();
execSync(
  'git commit -m "build: [skip ci] [skip release] update version" --no-verify',
);
execSync(
  `git remote set-url origin https://georgi.kotov:${process.env.AZURE_PAT_DECODED}@dev.azure.com/dssbg/NEISPUO%20Maintenance/_git/mon-neispuo-main-portal`,
);
execSync(`git push -u origin ${branchName}`);
