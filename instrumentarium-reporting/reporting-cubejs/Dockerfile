# Using  trixie-slim Node.js image instead of Alpine because Cube.js native backend extension
# (@cubejs-backend/native) doesn't support Alpine Linux (Musl libc). This image was chosen after testing
# multiple Node.js variants - all of the following work with the native backend extension:
# 22.18.0, 22.18.0-slim, 22.18.0-bullseye, 22.18.0-bullseye-slim, 22.18.0-bookworm, 22.18.0-bookworm-slim, 22.18.0-trixie-slim
# This trixie-slim variant offers the best balance of size and security for the LTS Node.js version
FROM node:22.18.0-trixie-slim

COPY . /reporting-cubejs/
WORKDIR /reporting-cubejs/

RUN mv /reporting-cubejs/.env-example-docker /reporting-cubejs/.env

## Build the project
RUN npm install
RUN npm install pm2 -g

## NODE_ENV - development/staging/production
ENV NODE_ENV=
ENV PORT=4000

# CUBE.js
ENV CUBEJS_DB_HOST=
ENV CUBEJS_DB_NAME=
ENV CUBEJS_DB_PORT=
ENV CUBEJS_DB_USER=
ENV CUBEJS_DB_PASS=
ENV CUBEJS_DEV_MODE=
ENV CUBEJS_DB_TYPE=
ENV CUBEJS_API_SECRET=
ENV CUBEJS_JWK_URL=
ENV CUBEJS_LOG_LEVEL=warn
ENV CUBEJS_REDIS_URL=
ENV CUBEJS_REDIS_PASSWORD=
ENV CUBEJS_REDIS_POOL_MIN=
ENV CUBEJS_REDIS_POOL_MAX=
ENV CUBEJS_TELEMETRY=false
ENV NODE_TLS_REJECT_UNAUTHORIZED=0
ENV REPORTING_BACKEND_SCHEMA_URL=
ENV REPORTING_BACKEND_X_API_KEY=
ENV CUBEJS_PRE_AGGREGATIONS_SCHEMA=
ENV CUBEJS_CUBESTORE_HOST=
ENV CUBEJS_CUBESTORE_PORT=
ENTRYPOINT ["pm2-runtime", "start", "ecosystem.config.js"]

EXPOSE 4000
