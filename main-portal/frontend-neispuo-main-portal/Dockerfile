FROM node:14.17.4-alpine3.14 as build
COPY ./ /main-portal/
WORKDIR /main-portal/

ENV BUILD_ENV=production
ENV APP_URL='http://localhost:4200'
ENV BACKEND_URL='http://localhost:3001'
ENV USER_MANAGMENT_BACKEND_URL='http://localhost:3005'
ENV HELP_DESK_URL='https://helpdesk-neispuo.mon.bg/'
ENV OIDC_BASE_URL='http://localhost:3000'
ENV OIDC_CLIENT_ID='neispuo-portal'
ENV ALLOWED_DOMAINS='localhost:3001,localhost:3005'
ENV DISALLOWED_ROUTES='http://localhost:3000'
ENV SECONDS_TO_CHECK_VERSION=300

RUN npm install
RUN npm run build -- --configuration=production --aot --outputHashing=all

FROM nginx:latest
COPY --from=build /main-portal/docker-entrypoint.sh /docker-entrypoint.d/docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.d/docker-entrypoint.sh

COPY --from=build  /main-portal/nginx.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build  /main-portal/dist/frontend-neispuo-main-portal/ /usr/share/nginx/html

RUN rm -rf /main-portal
EXPOSE 80
