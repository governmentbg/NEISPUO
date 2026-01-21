FROM node:14.17.4-alpine3.14 as build
COPY ./ /survey/
WORKDIR /survey/

RUN npm install
RUN npm run build -- --configuration=production --aot --outputHashing=all

FROM nginx:latest

ENV BUILD_ENV=production
ENV APP_URL='http://localhost:4203'
ENV BACKEND_URL='http://localhost:3003'
ENV OIDC_BASE_URL='http://localhost:3000'
ENV OIDC_CLIENT_ID='survey'
ENV ALLOWED_DOMAINS='localhost:3003'
ENV DISALLOWED_ROUTES='http://localhost:3000'

COPY --from=build /survey/docker-entrypoint.sh /docker-entrypoint.d/docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.d/docker-entrypoint.sh

COPY --from=build  /survey/nginx.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build  /survey/dist/survey/ /usr/share/nginx/html
RUN rm -rf /survey
EXPOSE 80
