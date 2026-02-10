#!/bin/sh
echo "$NEISPUO_ACR_PASS" | docker login neispuoprod.azurecr.io --username neispuoprod --password-stdin

image='students'

# Shut down running container
docker rm $image -f

GIT_COMMIT=`git rev-parse --short HEAD`
BUILD_DATE=`date +%Y%m%d%H%M%S`
BUILD_ENV=$1
TAG="${BUILD_ENV}-${BUILD_DATE}-${GIT_COMMIT}"

# Build the image
docker build -t ${image}:${TAG} --build-arg BUILD_ENV=${BUILD_ENV} --build-arg VERSION=${TAG} .
docker tag ${image}:${TAG} ${image}:${BUILD_ENV}-latest
docker tag ${image}:${BUILD_ENV}-latest neispuoprod.azurecr.io/${image}:${BUILD_ENV}-latest
docker tag ${image}:${TAG} neispuoprod.azurecr.io/${image}:${TAG}

# Run It
#docker run -it -p 44357:80 --rm --name $image $image

#Publish
docker push neispuoprod.azurecr.io/${image}:${BUILD_ENV}-latest
docker push neispuoprod.azurecr.io/${image}:${TAG}
