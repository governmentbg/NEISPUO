# Sample List (SO) - Frontend 

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 9.0.6.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

## Deployment instructions 

### Test environment 

```
docker buildx build -t neispuoprod.azurecr.io/so:test-latest -f Dockerfile-test . --platform=linux/amd64
docker push neispuoprod.azurecr.io/so:test-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = test 
deployment = so-test

### Production environment 

```
docker buildx build -t neispuoprod.azurecr.io/so:prd-latest -f Dockerfile-prod . --platform=linux/amd64
docker push neispuoprod.azurecr.io/so:prd-latest
```

You have to restart the deployment/pod in the Kubernetes cluster:
namespace = prd
deployment = so-prd

### ARM CPU

```
--platform=linux/amd64
```
