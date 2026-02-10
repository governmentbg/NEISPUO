# Admin Data interface - Frontend 

## Deployment instructions 

### Test environment 

```
docker build -t neispuoprod.azurecr.io/admin-data:test-latest -f Dockerfile-test .
docker push neispuoprod.azurecr.io/admin-data:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = admin-data-test

### Production environment 

```
docker build -t neispuoprod.azurecr.io/admin-data:latest -f Dockerfile-prod .
docker push neispuoprod.azurecr.io/admin-data:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = admin-data-prd

### ARM CPU

```
--platform=linux/amd64
```
