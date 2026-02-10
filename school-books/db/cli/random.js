const fs = require('fs');
const path = require('path');
const { isValid } = require('date-fns');
const _ = require('lodash');

const readAllLines = (filename) =>
  fs
    .readFileSync(path.resolve(__dirname, filename))
    .toString()
    .split('\n')
    .map((l) => l.trim())
    .filter((l) => l);

const getRandomItem = (arr) => arr[_.random(0, arr.length - 1)];

const maleNames = readAllLines('./male_names.txt');
const maleFamilies = readAllLines('./male_families.txt');
const femaleNames = readAllLines('./female_names.txt');
const femaleFamilies = readAllLines('./female_families.txt');
const families = readAllLines('./families.txt').map((l) =>
  l.match(/([а-яА-Я]+)\s*,\s*([а-яА-Я]+)\s*,\s*([а-яА-Я]+)/).slice(1)
);

const egnWeights = [2, 4, 8, 5, 10, 9, 7, 3, 6];

function randomDate({ day, mon, year } = {}) {
  if (year && (typeof year !== 'number' || year < 1800 || year > 2099)) {
    throw new Error('Invalid year');
  }

  if (mon && (typeof mon !== 'number' || mon < 1 || mon > 12)) {
    throw new Error('Invalid month');
  }

  if (day && (typeof day !== 'number' || day < 1 || day > 31)) {
    throw new Error('Invalid day');
  }

  if (day && mon && mon >= 0 && year && !isValid(new Date(year, mon - 1, day))) {
    throw new Error('Invalid day/month/year');
  }

  let gday;
  let gmon;
  let gyear;
  let iter = 0;
  do {
    gday = day || _.random(1, 31);
    gmon = mon || _.random(1, 12);
    gyear = year || _.random(1900, 2000);
    iter++;
  } while (!isValid(new Date(gyear, gmon - 1, gday)) && iter < 3);

  if (iter > 3) {
    throw new Error('Faild to generate random date');
  }

  return {
    day: gday,
    mon: gmon,
    year: gyear
  };
}

function randomEGN({ day, mon, year, male } = {}) {
  let { day: gday, mon: gmon, year: gyear } = randomDate({ day, mon, year });

  const cent = gyear - (gyear % 100);

  /* Fixes for other centuries */
  switch (cent) {
    case 1800:
      gmon += 20;
      break;
    case 2000:
      gmon += 40;
      break;
  }

  /* Generate region/sex */
  let gregion = _.random(0, 999);

  if (male && gregion % 2 != 0) {
    gregion--;
  }

  if (!male && gregion % 2 == 0) {
    gregion++;
  }

  /* Create EGN */
  let egn =
    _.padStart(gyear - cent, 2, '0') +
    _.padStart(gmon, 2, '0') +
    _.padStart(gday, 2, '0') +
    _.padStart(gregion, 3, '0');

  /* Calculate checksum */
  let egnsum = 0;
  for (let i = 0; i < 9; i++) {
    egnsum += parseInt(egn[i], 10) * egnWeights[i];
  }

  let valid_checksum = egnsum % 11;
  if (valid_checksum == 10) {
    valid_checksum = 0;
  }

  egn += valid_checksum;

  return egn;
}

module.exports.randomMaleName = () => getRandomItem(maleNames);
module.exports.randomMaleFamilyName = () => getRandomItem(maleFamilies);
module.exports.randomFemaleName = () => getRandomItem(femaleNames);
module.exports.randomFemaleFamilyName = () => getRandomItem(femaleFamilies);
module.exports.getRandomFamily = () => getRandomItem(families);
module.exports.randomDate = randomDate;
module.exports.randomEGN = randomEGN;
