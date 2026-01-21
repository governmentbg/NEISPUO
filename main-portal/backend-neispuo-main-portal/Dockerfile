FROM node:14.17.4-alpine3.14

COPY ./ /main-portal-be/
WORKDIR /main-portal-be/

## Build the project

RUN mv config/.env-example-docker config/.env 
RUN npm install
RUN npm install pm2 -g
RUN npm run build

## NODE_ENV - development/staging/production
ENV NODE_ENV=production
ENV PORT=3001

## DB
ENV DB_CLIENT=mssql
ENV DB_HOST=
ENV DB_PORT=
ENV DB_USERNAME=
ENV DB_PASSWORD=
ENV DB=

ENV FILE_STORAGE=files

## CORS
ENV CORS_DOMAINS=any
ENV CORS_METHODS="GET,PUT,POST,DELETE,PATCH,HEAD"
ENV CORS_ALLOWED_HEADERS=
ENV CORS_EXPOSED_HEADERS=
ENV CORS_CREDENTIALS=false
ENV CORS_MAX_AGE=
ENV CORS_PREFLIGHT_CONTINUE=false
ENV CORS_OPTIONS_SUCCESS_STATUS=204

## JWKS
ENV JWKS_ENDPOINT=

# used for internal unauthenticated requests between BE services
ENV INTERNAL_API_KEY=de67500e-37d2-4507-8061-18bb35b9210c

ENV USERMNG_SERVER_URL=https://usermng-server.mon.bg
ENV CREATE_PARENT_URI=/v1/azure-integrations/parent

EXPOSE 3001
ENTRYPOINT ["pm2-runtime", "start", "ecosystem.config.js"]
