# Sample List (SO) - Backend 

Run NEISPUORegInstAPI.sln 

## Deployment instructions 

### Test environment 

```
docker buildx build -t neispuoprod.azurecr.io/so-api:test-latest -f Dockerfile-test . --platform=linux/amd64
docker push neispuoprod.azurecr.io/so-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = so-api-test

### Production environment 

```
docker buildx build -t neispuoprod.azurecr.io/so-api:latest -f Dockerfile-prod . --platform=linux/amd64
docker push neispuoprod.azurecr.io/so-api:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = so-api-prd

### ARM CPU

```
--platform=linux/amd64
```
