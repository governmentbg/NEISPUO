# Register Stamps

## Deployment instructions 

### Test environment 

```
docker build -t neispuoprod.azurecr.io/regstamps:test-latest -f RegStamps.Web/Dockerfile-test .
docker push neispuoprod.azurecr.io/regstamps:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = regstamps-test

### Production environment 

```
docker build -t neispuoprod.azurecr.io/regstamps:latest -f RegStamps.Web/Dockerfile-prod .
docker push neispuoprod.azurecr.io/regstamps:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = regstamps-prd

### ARM CPU

```
--platform=linux/amd64
```
