# Regix Integration

## Deployment instructions 

### Dev environment 

```
cd C:\github\institutions\RegixApi
git pull
npm run start:dev
```
The application runs on http://10.10.4.40:7180/ and https://10.10.4.40:7143/
### Test environment 

```
docker build -t neispuoprod.azurecr.io/regix-api:test-latest -f Dockerfile-test .
docker push neispuoprod.azurecr.io/regix-api:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = regix-api-test

### Production environment 

```
docker build -t neispuoprod.azurecr.io/regix-api:latest -f Dockerfile-prod .
docker push neispuoprod.azurecr.io/regix-api:latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = regix-api-prd

### ARM CPU

```
--platform=linux/amd64
```
