#!/bin/sh
CONF_PATH=/etc/nginx/conf.d/default.conf
APP_DIR=/usr/share/nginx/html/
PREVIOUS_VERSIONS_JS=/usr/share/nginx/prev/
APP_ENV_VAR_PREFIX=SB__
APP_ENV_VAR_NAMES=$(env | cut -d= -f1 | grep $APP_ENV_VAR_PREFIX | sed -e 's/^/$/')

##################################################################
# Prepare the application for the environment this container is in
##################################################################

# in-place replace the application env vars in the nginx configuration file with envsubst
{ rm $CONF_PATH; envsubst "$APP_ENV_VAR_NAMES" > $CONF_PATH; } < $CONF_PATH;

# in-place replace the application env vars in the application files (js & html) with envsubst
# https://backreference.org/2011/01/29/in-place-editing-of-files/
for f in $(find $APP_DIR -type f \( -iname \*.js -o -iname \*.html -o -iname VERSION \))
do
  { rm $f; envsubst "$APP_ENV_VAR_NAMES" > $f; } < $f
done

# Gzip all js/html/css files
find $APP_DIR -type f \( -iname \*.js -o -iname \*.html -o -iname \*.css \) -print0 | xargs -0r gzip -9 -k -f

# Reset all mofification timestamps to be the same as teacher's favicon.ico
touch -r "$APP_DIR/t/favicon.ico" "$APP_DIR"
find $APP_DIR -type f \( -iname \*.js -o -iname \*.html -o -iname \*.css -o -iname \*.gz \) -print0 | xargs -0 touch -r "$APP_DIR/t/favicon.ico"

##################################################################
# Copy the previous versions js/gz files and delete the ones
# older than 5 versions ago
##################################################################

if [ ! -d $PREVIOUS_VERSIONS_JS ]; then
  echo "Previous versions js folder not mounted. Skipping previous versions copying."
  exit 0
fi

DEPL_COPY="$PREVIOUS_VERSIONS_JS/$SB__FRONTEND__APP__VERSION"

if [ -d $DEPL_COPY ]; then
  echo "Deployment already copied. Skipping copying deployment to previous versions folder."
else
  echo -n "Copying deployment to previous versions folder... "

  mkdir $DEPL_COPY
  touch -r "$APP_DIR/t/favicon.ico" $DEPL_COPY

  for DIR in "$APP_DIR"*/; do
      BASE_DIR=$(basename $DIR)
      mkdir "$DEPL_COPY/$BASE_DIR"

      # copy all js/css files without recursion
      find $DIR -maxdepth 1 \
        \( -name '*.js' -o -name '*.js.gz' -o -name '*.js.map' \
        -o -name '*.css' -o -name '*.css.gz' -o -name '*.css.map' \) \
        -exec cp {} "$DEPL_COPY/$BASE_DIR" \;
  done

  echo "Done."

  echo -n "Deleting all but the last 5 version... "

  cd $PREVIOUS_VERSIONS_JS
  ls -tp | grep '/$' | tail -n +6 | xargs -I {} rm -rf -- {}

  echo "Done."
fi

echo -n "Copying all previous versions to the app folder... "

for COPY_DIR in "$PREVIOUS_VERSIONS_JS"*/; do
  for DIR in "$COPY_DIR"*/; do
    BASE_DIR=$(basename $DIR)

    # copy all js/css files without recursion and overwriting
    find $DIR -maxdepth 1 \
      \( -name '*.js' -o -name '*.js.gz' -o -name '*.js.map' \
      -o -name '*.css' -o -name '*.css.gz' -o -name '*.css.map' \) \
      -exec cp -n {} "$APP_DIR/$BASE_DIR" \;
  done
done

echo "Done."
