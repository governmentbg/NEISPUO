FROM node:14.17.4-alpine3.14 as build

COPY ./ /reporting-frontend/
WORKDIR /reporting-frontend/

ENV BUILD_ENV=production
ENV APP_URL='http://localhost:4200'
ENV BACKEND_URL='http://localhost:3001'
ENV MAIN_PORTAL_URL='http://localhost:4201'
ENV OIDC_BASE_URL='http://localhost:3000'
ENV OIDC_CLIENT_ID='neispuo-reporting'
ENV ALLOWED_DOMAINS='localhost:3001,localhost:4000'
ENV DISALLOWED_ROUTES='http://localhost:3000'
ENV HELPDESK_URL='https://helpdesk-neispuo.mon.bg/'
ENV CUBEJS_API_URL='http://localhost:3001/v1/cubejs'

RUN npm install
RUN npm run build -- --configuration=production --aot --outputHashing=all

FROM nginx:latest
COPY --from=build /reporting-frontend/docker-entrypoint.sh /docker-entrypoint.d/docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.d/docker-entrypoint.sh

COPY --from=build  /reporting-frontend/nginx.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build  /reporting-frontend/dist/reporting-frontend/ /usr/share/nginx/html

RUN rm -rf /reporting-frontend
EXPOSE 80
