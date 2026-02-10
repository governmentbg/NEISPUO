## Installation instructions
1. Open a terminal in the root directory and execute the following command
npm install
1. Open environments.ts file and make sure that the OIDC and backendconfigs are ok.
1. Make sure to check if the OIDC project is configured and bootstraped.
1. Check the OIDC project app/support/configuration.js file if this project is correctly setup as a client.
1. Execute the following command in terminal
1. Copy the desired configuration from environment.ts to config.json in assets/config/config.json
1. Run `ng server --port=4205`

## Docker build steps for MON Kubernetes Dev env

1. Update Dockerfile 
1. Run `docker build --no-cache -t usermng-fe . --build-arg BUILD_ENV=dev`
1. Run `docker tag usermng-fe:latest neispuoprod.azurecr.io/usermng-fe`
1. Run `docker push neispuoprod.azurecr.io/usermng-fe`
