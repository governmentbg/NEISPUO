# syntax=docker/dockerfile:1

ARG NODE_VERSION=16-alpine
ARG NGINX_VERSION=stable-alpine

# Build stage, independent cache
FROM node:$NODE_VERSION AS build-env
WORKDIR /app

# Define arguments for this build step as FROM resets all arguments
# https://docs.docker.com/engine/reference/builder/#understand-how-arg-and-from-interact
ARG SSH_KEY_PUB
ARG SSH_KEY_BASE64

# Required for timezone support
RUN apk add --no-cache tzdata

# Add mising utilities
RUN apk add --no-cache git openssh-client

# Required for @parcel/watcher@2.0.4(dep of @angular-eslint/builder@14.0.2)
# on arm64 alpine as there is no prebuild binary for it
# RUN apk add --no-cache g++ gcc libgcc libstdc++ linux-headers make python3 git fts-dev

# Install Chrome
RUN apk add --no-cache chromium
ENV CHROME_BIN=/usr/bin/chromium-browser \
    CHROME_PATH=/usr/lib/chromium/

# Authorize SSH Host
RUN mkdir -p /root/.ssh && \
    chmod 0700 /root/.ssh && \
    ssh-keyscan github.com > /root/.ssh/known_hosts

# Add the keys and set permissions
# the ssh key is base64 encoded as Github Actions has a problem with multiline strings
RUN echo "$SSH_KEY_PUB" > /root/.ssh/id_rsa.pub && \
    echo "$SSH_KEY_BASE64" | base64 -d > /root/.ssh/id_rsa && \
    chmod 600 /root/.ssh/id_rsa.pub && \
    chmod 600 /root/.ssh/id_rsa

# Copy only package.json and package-lock.json
# then install packages as a distinct layer
COPY package.json package-lock.json ./
RUN npm ci

# Copy everything else and lint/build
COPY ./ ./
RUN npm run lint
RUN npm run test:ci
RUN npm run build teacher
RUN npm run build student

ARG APP_VERSION

# Create VERSION file
RUN echo "$APP_VERSION" >> /app/VERSION && \
    echo "\$SB__FRONTEND__ENV" >> /app/VERSION

# Build runtime image for frontend
FROM nginx:$NGINX_VERSION AS frontend

#Set app version in environment variables
ARG APP_VERSION
ENV SB__FRONTEND__APP__VERSION=$APP_VERSION

# Set linux timezone
ARG TZ=Europe/Sofia
COPY --from=build-env /usr/share/zoneinfo/$TZ /etc/localtime
RUN echo "$TZ" > /etc/timezone

RUN rm -rf /usr/share/nginx/html/*
COPY --from=build-env /app/dist/teacher /usr/share/nginx/html/t/
COPY --from=build-env /app/dist/student /usr/share/nginx/html/s/
COPY --from=build-env /app/VERSION /usr/share/nginx/html/
COPY --chmod=775 ./docker-start.sh /
COPY ./docker-nginx-default.conf /etc/nginx/conf.d/default.conf
CMD /bin/sh -c "/docker-start.sh && nginx -g 'daemon off;'"
