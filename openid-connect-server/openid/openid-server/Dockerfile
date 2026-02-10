FROM node:14.17.4-alpine3.14
COPY ./ /oidc/
WORKDIR /oidc/

RUN npm rebuild node-sass
RUN npm install pm2 -g
RUN npm install

## APP_ENV - points to which oidc-client-apps.config will be used based on environemnt. Currently dev/test/prod
ENV APP_ENV=dev

## NODE_ENV - development/production
ENV NODE_ENV=production
ENV ROOT_URI=
ENV PORT=3000

## DB
ENV MSSQL_HOST=
ENV MSSQL_PORT=
ENV MSSQL_DB=
ENV MSSQL_USER=
ENV MSSQL_PASS=

## Forgot-password
ENV BCRYPT_SALT_ROUNDS=12
ENV FORGOT_PASSWORD_TOKEN_EXPIRATION_MESSAGE='Линкът е активен в следващите 48 часа.'
ENV FORGOT_PASSWORD_TOKEN_EXPIRATION_IN_SECONDS=172800

## Redis
ENV REDIS_ON=true
ENV REDIS_URL=redis-dev.dev.svc.mon.local:6379

## Azure
ENV CLIENT_DISCOVERY=https://login.microsoftonline.com/420584ab-4eec-41c7-bb43-7364d0a6fdfd/v2.0/.well-known/openid-configuration
ENV CLIENT_ID=3fd59152-3047-4044-8383-4154c3498778
ENV CLIENT_SECRET=test
ENV AZURE_CHALLENGE_EXPIRATION=900

## Azure parent integration
ENV PARENT_CLIENT_DISCOVERY="https://mdutb2c.b2clogin.com/mdutb2c.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_SignIn"
ENV PARENT_CLIENT_ID="7ef806bf-406c-4dc1-9911-8fa1e722c815"
ENV PARENT_CLIENT_SECRET="test2"

## Recaptcha
ENV RECAPTCHA_ON=false
ENV RECAPTCHA_SITE_KEY=
ENV RECAPTCHA_SECRET=

## Main Portal
ENV MAIN_PORTAL_URL=

## OIDC
ENV JWT_EXPIRATION_IN_S=1200

ENV LOCAL_LOGIN_WHITELIST='::1,127.0.0.1,0.0.0.0'

## IMPERSONATE
### can only hold 1 url for security reasons - otherwise client session cannot be sent
ENV IMPERSONATE_ORIGIN_WHITELIST='http://localhost:4200'

ENV IMPERSONATION_CLIENT='user-management'

## Test accounts with multiple roles
ENV IGNORE_USERS_LIST=neispuo_dss@edu.mon.bg,neispuo_tu@edu.mon.bg,neispuo_kontrax@edu.mon.bg,neispuo_adminsoft@edu.mon.bg,neispuo_ciela@edu.mon.bg,neispuo_mon_security@edu.mon.bg

RUN chmod +x /oidc/docker-entrypoint.sh

EXPOSE 3000

ENTRYPOINT ["./docker-entrypoint.sh"]