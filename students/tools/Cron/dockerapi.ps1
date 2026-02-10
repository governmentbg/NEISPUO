param ($push)
write-host "Push to registry: $push"
if ('true' -eq $push){
  echo "$Env:NEISPUO_ACR_PASS" | docker login neispuoprod.azurecr.io --username neispuoprod --password-stdin
}

$image='students-cron'

# Shut down running container
docker rm $image -f

$GIT_COMMIT=git rev-parse --short HEAD
$BUILD_DATE=Get-Date -Format "yyyyMMddHHmmss"
$TAG=$BUILD_DATE+'-'+$GIT_COMMIT

# Build the image
docker build -t ${image}:${TAG} --build-arg VERSION=${TAG} .
docker tag ${image}:${TAG} ${image}:latest
if ('true' -eq $push){
   docker tag ${image}:latest neispuoprod.azurecr.io/${image}:latest
   docker tag ${image}:${TAG} neispuoprod.azurecr.io/${image}:${TAG}
}

#Publish
if ('true' -eq $push){
  docker push neispuoprod.azurecr.io/${image}:latest
  docker push neispuoprod.azurecr.io/${image}:${TAG}
}