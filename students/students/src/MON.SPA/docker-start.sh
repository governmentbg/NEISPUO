#!/bin/sh
APP_DIR=/usr/share/nginx/html/
APP_ENV_VAR_PREFIX=ST__
APP_ENV_VAR_NAMES=$(env | cut -d= -f1 | grep $APP_ENV_VAR_PREFIX | sed -e 's/^/$/')

# in-place replace the application env vars in the application files (js & html) with envsubst
# https://backreference.org/2011/01/29/in-place-editing-of-files/
for f in $(find $APP_DIR -type f \( -iname \*.js -o -iname \*.html \))
do
  { rm $f; envsubst "$APP_ENV_VAR_NAMES" > $f; } < $f
done

# Gzip all js/html/css files
find $APP_DIR -type f \( -iname \*.js -o -iname \*.html -o -iname \*.css \) -print0 | xargs -0r gzip -9 -k -f

# Reset all mofification timestamps to be the same as favicon.ico
find $APP_DIR -type f \( -iname \*.js -o -iname \*.html -o -iname \*.css -o -iname \*.gz \) -print0 | xargs -0 touch -r "$APP_DIR/favicon.ico"