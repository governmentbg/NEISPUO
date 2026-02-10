#!/bin/sh

mv /oidc/app/support/oidc-client-apps.config_${APP_ENV}.json /oidc/app/support/oidc-client-apps.config.json
pm2-runtime start ecosystem.config.js
