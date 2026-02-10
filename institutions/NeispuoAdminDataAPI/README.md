# Admin Data interface - Backend 

Run NEISPUORegInstAPI.sln 

## Deployment instructions 

### Test environment 

```
docker build -t neispuoprod.azurecr.io/admin-data-api:test-latest -f Dockerfile-test .
docker push neispuoprod.azurecr.io/admin-data-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = admin-data-api-test

### Production environment 

```
docker build -t neispuoprod.azurecr.io/admin-data-api:latest -f Dockerfile-prod .
docker push neispuoprod.azurecr.io/admin-data-api:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = admin-data-api-prd

### ARM CPU

```
--platform=linux/amd64
```
