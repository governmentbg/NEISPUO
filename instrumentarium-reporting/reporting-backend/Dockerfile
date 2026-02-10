FROM node:14.17.4-alpine3.14

COPY . /reporting-backend/
WORKDIR /reporting-backend/

RUN mv /reporting-backend/.env-example-docker /reporting-backend/config/.env

## Build the project
RUN npm install
RUN npm install pm2 -g
RUN npm run build


## NODE_ENV - development/staging/production
ENV NODE_ENV=
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

ENV CUBEJS_ENDPOINT=

## JWKS
ENV JWKS_ENDPOINT=

ENTRYPOINT ["pm2-runtime", "start", "ecosystem.config.js"]

EXPOSE 3001
