IS_PROD=false

echo "Configuring config.json file..."

if [ "$BUILD_ENV" = "production" ]; then
    IS_PROD=true
fi

config="\
{\
    \"production\": $IS_PROD,\
    \"APP_URL\": \"$APP_URL\",\
    \"BACKEND_URL\": \"$BACKEND_URL\",\
    \"USER_MANAGMENT_BACKEND_URL\": \"$USER_MANAGMENT_BACKEND_URL\",\
    \"HELP_DESK_URL\": \"$HELP_DESK_URL\",\
    \"OIDC_BASE_URL\": \"$OIDC_BASE_URL\",\
    \"OIDC_CLIENT_ID\": \"$OIDC_CLIENT_ID\",\
    \"ALLOWED_DOMAINS\": \"$ALLOWED_DOMAINS\",\
    \"DISALLOWED_ROUTES\": \"$DISALLOWED_ROUTES\",\
    \"SECONDS_TO_CHECK_VERSION\": \"$SECONDS_TO_CHECK_VERSION\"\
}\
"

echo $config > 'usr/share/nginx/html/assets/config/config.json'

echo "Configuring NGINX default.conf file..."
CONNECT_SRC="${BACKEND_URL} ${OIDC_BASE_URL} ${USER_MANAGMENT_BACKEND_URL}"
FRAME_SRC="${OIDC_BASE_URL}"
sed -i -e "s|connect-src-array|$CONNECT_SRC|gi" /etc/nginx/conf.d/default.conf
sed -i -e "s|frame-src-array|$FRAME_SRC|gi" /etc/nginx/conf.d/default.conf


