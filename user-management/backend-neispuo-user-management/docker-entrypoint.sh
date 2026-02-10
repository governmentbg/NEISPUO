#!/bin/sh
if [ "$AUTO_RUN_MIGRATIONS" = "true" ]; then
   npm run db:migrate
   echo $?
fi

pm2-runtime start ecosystem.config.js