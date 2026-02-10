const gulp = require('gulp');
const util = require('gulp-util'); // https://github.com/gulpjs/gulp-util
const sass = require('gulp-sass'); // https://www.npmjs.org/package/gulp-sass
const autoprefixer = require('gulp-autoprefixer'); // https://www.npmjs.org/package/gulp-autoprefixer
const minifycss = require('gulp-minify-css'); // https://www.npmjs.org/package/gulp-minify-css
const rename = require('gulp-rename'); // https://www.npmjs.org/package/gulp-rename
const babel = require('gulp-babel');
const browserify = require('browserify');
const tap = require('gulp-tap');
const buffer = require('gulp-buffer');
const del = require('del');

sass.compiler = require('sass');

const { log } = util;

gulp.task('clean:css', () => {
  log(`Deleting previous css build ${new Date().toString()}`);
  return del(['./app/views/dist/css/**/*']);
});

gulp.task('build:css', () => {
  log(`Generating CSS files ${new Date().toString()}`);
  return (
    gulp
      .src('./app/views/scss/**/*.scss')
      .pipe(sass({ includePaths: ['node_modules'] }))
      .pipe(autoprefixer('last 3 version', 'safari 5' /* , 'ie 8', 'ie 9' */))
      // .pipe(gulp.dest('./app/views/dist/css'))
      .pipe(rename({ suffix: '.min' }))
      .pipe(minifycss())
      .pipe(gulp.dest('./app/views/dist/css'))
  );
});

gulp.task('copy:fonts', () => {
  return gulp
    .src('./node_modules/bootstrap-icons/font/fonts/*')
    .pipe(gulp.dest('./app/views/dist/css/neispuo-theme-styles/fonts'));
});

gulp.task('clean:js', () => {
  log(`Deleting previous js build and dependencies ${new Date().toString()}`);
  return del(['./app/views/dist/js/**/*']);
});

gulp.task('build:js', () => {
  log(`Building previous js build and dependencies ${new Date().toString()}`);

  return gulp
    .src(['./app/views/js/*.js'])
    .pipe(
      babel({
        // https://babeljs.io/docs/en/babel-preset-env
        presets: [
          [
            '@babel/preset-env',
            {
              // targets: { // This also works
              //   chrome: '58',
              //   // ie: '11',
              // },
              targets: '> 0.25%, not dead',
              useBuiltIns: 'usage',
              corejs: 3,
            },
          ],
        ],
      }),
    )
    .pipe(gulp.dest('./app/views/dist/js'));
});

// src: https://github.com/gulpjs/gulp/blob/master/docs/recipes/browserify-multiple-destination.md
gulp.task('browserify', () => {
  log(`Running browserify ${new Date().toString()}`);
  return (
    gulp
      .src('./app/views/dist/js/*.js', { read: false }) // no need of reading file because browserify does.

      // transform file objects using gulp-tap plugin
      .pipe(
        tap((file) => {
          log(`bundling ${file.path}`);

          // replace file contents with browserify's bundle stream
          file.contents = browserify(file.path, { debug: false }).bundle();
        }),
      )

      .pipe(buffer())

      .pipe(gulp.dest('./app/views/dist/js'))
  );
});

gulp.task(
  'default',
  gulp.series(
    'clean:css',
    'build:css',
    'copy:fonts',
    'clean:js',
    'build:js',
    'browserify',
  ),
);
