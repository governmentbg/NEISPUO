npm run ng build -- --configuration=test
docker build -t neispuoprod.azurecr.io/so:test-latest .
docker push neispuoprod.azurecr.io/so:test-latest

npm run ng build -- --prod &
docker build -t neispuoprod.azurecr.io/so:prd-latest .
docker push neispuoprod.azurecr.io/so:prd-latest
