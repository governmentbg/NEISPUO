npm run ng build
docker build -t neispuoprod.azurecr.io/institutions:dev-latest .
docker login neispuoprod.azurecr.io --username neispuoprod
docker push neispuoprod.azurecr.io/institutions:dev-latest

npm run ng build -- --configuration=test
docker build -t neispuoprod.azurecr.io/institutions:test-latest .
docker push neispuoprod.azurecr.io/institutions:test-latest

npm run ng build -- --prod &
docker build -t neispuoprod.azurecr.io/institutions:prd-latest .
docker login neispuoprod.azurecr.io --username neispuoprod
docker push neispuoprod.azurecr.io/institutions:prd-latest
