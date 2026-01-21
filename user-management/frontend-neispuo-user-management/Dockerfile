FROM node:14.17.4-alpine3.14 as build

COPY ./ /usermng/
WORKDIR /usermng/

ENV BUILD_ENV=production
ENV APP_URL='http://localhost:4205'
ENV BACKEND_URL='http://localhost:3005'
ENV MAIN_PORTAL_URL='http://localhost:4201'
ENV OIDC_BASE_URL='http://localhost:3000'
ENV OIDC_CLIENT_ID='user-management'
ENV ALLOWED_DOMAINS='localhost:3005'
ENV DISALLOWED_ROUTES='http://localhost:3000'
ENV HELPDESK_URL='https://helpdesk-neispuo.mon.bg/'

RUN npm install
RUN npm run build -- --prod --aot --outputHashing=all

FROM nginx:latest
COPY --from=build /usermng/docker-entrypoint.sh /docker-entrypoint.d/docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.d/docker-entrypoint.sh

COPY --from=build  /usermng/nginx.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build  /usermng/dist/user-management-front-end/ /usr/share/nginx/html

RUN rm -rf /usermng
EXPOSE 80
