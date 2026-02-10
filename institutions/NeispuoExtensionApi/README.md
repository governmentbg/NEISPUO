# Sample List (SO) - Backend 

Run NEISPUORegInstAPI.sln 

## Deployment instructions 

### Test environment 

```
docker buildx build -t neispuoprod.azurecr.io/extension-api:test-latest -f Dockerfile-test . --platform=linux/amd64
docker push neispuoprod.azurecr.io/extension-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = extension-api-test

### Production environment 

```
docker buildx build -t neispuoprod.azurecr.io/extension-api:latest -f Dockerfile-prod . --platform=linux/amd64
docker push neispuoprod.azurecr.io/extension-api:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = extension-api-prd

### ARM CPU

```
--platform=linux/amd64
```
