# Register Institutions - Backend


Run NEISPUORegInstAPI.sln 

## Deployment instructions 

### Test environment 

```
docker build -t neispuoprod.azurecr.io/institutions-api:test-latest -f Dockerfile-test .
docker push neispuoprod.azurecr.io/institutions-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = institutions-api-test

### Production environment 

```
docker build -t neispuoprod.azurecr.io/institutions-api:latest -f Dockerfile-prod .
docker push neispuoprod.azurecr.io/institutions-api:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = institutions-api-prd

### ARM CPU

```
--platform=linux/amd64
```
