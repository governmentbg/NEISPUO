date

docker kill oidc
docker rm oidc
docker image rm oidc
docker build --no-cache -t oidc .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run -d --name oidc -p 3000:3000 -p 9229:9229 oidc
date
exit 0

## Build production
# docker build --no-cache -t oidc:1.0.0 . -f Dockerfile_prod && docker tag oidc:1.0.0 neispuoprod.azurecr.io/oidc:1.0.0 && docker push neispuoprod.azurecr.io/oidc:1.0.0

## Build dev
# docker build --no-cache -f Dockerfile_dev -t oidc:latest . && docker tag oidc:latest neispuoprod.azurecr.io/oidc && docker push neispuoprod.azurecr.io/oidc