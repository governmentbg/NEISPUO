FROM node:14.17.4-alpine3.14 as build
COPY ./ /rmi/
WORKDIR /rmi/

ENV BUILD_ENV=production
ENV APP_URL='http://localhost:4200'
ENV BACKEND_URL='http://localhost:3001'
ENV OIDC_BASE_URL='http://localhost:3000'
ENV OIDC_CLIENT_ID='test-rmi'
ENV BLOB_SERVER_URL='https://test-blobs.mon.bg'
ENV ALLOWED_DOMAINS='localhost:3001'
ENV DISALLOWED_ROUTES='http://localhost:3000'
ENV HELPDESK_URL='https://helpdesk-neispuo.mon.bg/'
ENV MAIN_PORTAL_URL='https://neispuo.mon.bg/'

RUN npm install
RUN npm run config
RUN npm run build -- --prod --aot --outputHashing=all

FROM nginx:latest

COPY --from=build /rmi/docker-entrypoint.sh /docker-entrypoint.d/docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.d/docker-entrypoint.sh

COPY --from=build  /rmi/nginx.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build  /rmi/dist/rmi/ /usr/share/nginx/html
RUN rm -rf /rmi
EXPOSE 80
