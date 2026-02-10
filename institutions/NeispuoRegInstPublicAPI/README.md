# Register Institutions - Public Backend


Run NEISPUORegInstAPI.sln 

## Deployment instructions 

### Test environment 

```
docker build -t neispuoprod.azurecr.io/ri-api:test-latest -f Dockerfile-test .
docker push neispuoprod.azurecr.io/ri-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = ri-api-test

### Production environment 

```
docker build -t neispuoprod.azurecr.io/ri-api:latest -f Dockerfile-prod .
docker push neispuoprod.azurecr.io/ri-api:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = ri-api-prd

### ARM CPU

```
--platform=linux/amd64
```
