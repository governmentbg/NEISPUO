ARG NODE_VERSION=22-alpine

# Production node_modules stage
FROM node:$NODE_VERSION AS production-modules-env

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci --omit=dev

# Build stage
FROM node:$NODE_VERSION AS build-env

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

RUN npm run build
RUN npm run lint

# Runtime image
FROM node:$NODE_VERSION AS logger

WORKDIR /app

COPY --from=production-modules-env /app ./

COPY --from=build-env /app/dist ./

ARG APP_VERSION
RUN echo "$APP_VERSION" >> /app/VERSION

ENTRYPOINT ["node", "index.js"]
