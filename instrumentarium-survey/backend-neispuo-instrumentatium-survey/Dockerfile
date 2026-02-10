FROM node:14.17.4-alpine3.14

COPY ./ /backend-neispuo-instrumentatium-survey/
WORKDIR /backend-neispuo-instrumentatium-survey/

## Build the project

RUN mv config/.env-example-docker config/.env
RUN npm install
RUN npm install pm2 -g
RUN npm run prebuild
RUN npm run build

## NODE_ENV - development/staging/production
ENV ENV=production
ENV APP_PORT=3001

## DB

ENV DB_CLIENT=mssql
ENV DB_HOST=
ENV DB_PORT=
ENV DB_USERNAME=
ENV DB_PASSWORD=
ENV DB=

## CORS
ENV CORS_DOMAINS=any
ENV CORS_METHODS="GET,PUT,POST,DELETE,PATCH,HEAD"
ENV CORS_ALLOWED_HEADERS=
ENV CORS_EXPOSED_HEADERS=
ENV CORS_CREDENTIALS=false
ENV CORS_MAX_AGE=
ENV CORS_PREFLIGHT_CONTINUE=false
ENV CORS_OPTIONS_SUCCESS_STATUS=204

# Email
ENV EMAIL_FROM=trademarket.mon@gmail.com
ENV EMAIL_HOST=smtp.gmail.com
ENV EMAIL_PORT=465
ENV EMAIL_POOL=true
ENV EMAIL_USER=trademarket.mon
ENV EMAIL_PASSWORD=SuperSecret123!@#
ENV EMAIL_SECURE=true
ENV STOP_EMAILS=false

ENV MAX_BODY_SIZE_MB=15

## EMAIL OVERRIDES
### Send all mails to DEV_INBOX if it is set
ENV DEV_INBOX=rbhbkk23@hotmail.red

## JWKS
ENV JWKS_ENDPOINT=

# JWT_EXPIRATION_SECONDS 8h
ENV JWT_EXPIRATION_SECONDS=28800
ENV JWT_CERT_PATH=config/jwt-certificate.pem

# Whitelisted IPS for access from ESUI
ENV ESUI_ACCESS_IPS=

EXPOSE 3001
ENTRYPOINT ["pm2-runtime", "start", "ecosystem.config.js"]


