# Sample List (SO) External - Backend 


## Deployment instructions 

### Test environment 

```
docker buildx build -t neispuoprod.azurecr.io/so-ext-api:test-latest -f Dockerfile-test .
docker push neispuoprod.azurecr.io/so-ext-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = so-ext-api-test

### Production environment 

```
docker buildx build -t neispuoprod.azurecr.io/so-ext-api:prd-latest -f Dockerfile-prod .
docker push neispuoprod.azurecr.io/so-ext-api:prd-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = so-ext-api-prd

### ARM CPU

```
--platform=linux/amd64
```
