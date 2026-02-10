FROM node:18-alpine as docs-build-stage
ARG BUILD_ENV=production
ARG VERSION
ENV VERSION=$VERSION

WORKDIR /app
COPY docs/package.json ./
RUN npm install

COPY docs/. .
env VITEPRESS_ENV=${BUILD_ENV}
RUN npm run docs:build

FROM node:16-alpine as build-stage
ARG BUILD_ENV
ENV BUILD_ENV=$BUILD_ENV
ARG VERSION
ENV VERSION=$VERSION

WORKDIR /app
COPY package.json ./
RUN npm install
COPY . .

# build app for production with minification
RUN npm run build -- --mode ${BUILD_ENV}

# production stage
FROM nginx:stable-alpine as production-stage
ARG BUILD_ENV
ENV BUILD_ENV=$BUILD_ENV
ARG VERSION
ENV VERSION=$VERSION
COPY --from=build-stage /app/dist /app 
COPY --from=docs-build-stage /app/.vitepress/dist /app/docs
RUN echo $VERSION > /app/version

#/usr/share/nginx/html
COPY nginx.conf.${BUILD_ENV} /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
