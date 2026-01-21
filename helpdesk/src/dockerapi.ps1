echo "$Env:NEISPUO_ACR_PASS" | docker login neispuoprod.azurecr.io --username neispuoprod --password-stdin

$image='helpdesk-api'

# Shut down running container
docker rm $image -f

$GIT_COMMIT=git rev-parse --short HEAD
$BUILD_DATE=Get-Date -Format "yyyyMMddHHmmss"
$TAG=$BUILD_DATE+'-'+$GIT_COMMIT

# Build the image
docker build -t ${image}:${TAG} --build-arg VERSION=${TAG} .
docker tag ${image}:${TAG} ${image}:latest
docker tag ${image}:latest neispuoprod.azurecr.io/${image}:latest
docker tag ${image}:${TAG} neispuoprod.azurecr.io/${image}:${TAG}

# Run it
#docker run -d -p 44398:80 --name $image $image

#Publish
docker push neispuoprod.azurecr.io/${image}:latest
docker push neispuoprod.azurecr.io/${image}:${TAG}
