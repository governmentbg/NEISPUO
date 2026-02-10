FROM node:14-alpine as build-stage
ARG BUILD_ENV=production
ARG VERSION
ENV VERSION=$VERSION

# make the 'app' folder the current working directory
WORKDIR /app

# copy both 'package.json' and 'package-lock.json' (if available)
COPY package.json ./

# install project dependencies
RUN npm install

# copy project files and folders to the current working directory (i.e. 'app' folder)
COPY . .

# build app for production with minification
RUN npm run build -- --mode ${BUILD_ENV}

# production stage
FROM nginx:stable-alpine as production-stage
ARG VERSION
ENV VERSION=$VERSION
COPY --from=build-stage /app/dist /app
RUN echo $VERSION > /app/version

#/usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
