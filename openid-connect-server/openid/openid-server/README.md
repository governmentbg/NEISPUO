# Node OpenId Connect

This project is a fork of [node-oidc-provider](https://github.com/panva/node-oidc-provider) version 6.29.7.

Little (if any) changes were made to the `/lib` folder.

Most changes made were related to working around the restrictive API around consent and views.

For future development, [their example folder](https://github.com/panva/node-oidc-provider/tree/master/example) is a good place to start.

## Documentation

### Overview

Overall, the project follows the same structure as the original `/example` folder from [node-oidc-provider](https://github.com/panva/node-oidc-provider) version 6.29.7. Here is an overview of the changes made:

1. Deleted unused files (eg. example Koa routers were removed since we are using express)
2. Added `/app/integrations` folder, and new files under `/app/views` and `/app/support` (described later on in this document).
3. Avoided renaming any files, to facilitate mental-mapping in future updates.
4. Avoided making changes under `/lib`, but some small changes were unavoidable:
   1. check session iframe needed adjustment
   2. needed to add custom RoleRevokedError under `/lib/shared/error_handler.js`
   3. other small changes may have been necessary. If you need to know all of them, it is suggested that you diff the current `/lib` folder with the original `/lib` from [node-oidc-provider](https://github.com/panva/node-oidc-provider) version 6.29.7.

### Deployment

When deploying this application there are 4 places where you may need to make changes:

1. /.env
1. /Dockerfile
2. /app/support/example-oidc-client-apps.config.json
3. /app/support/cookie-keys-prod.js
4. /app/support/jwks-prod.js

More instructions may be found bellow.

#### Prerequisites for running the project
1. You will need to have setup a MSSQL database with the NEISPUO schema and pre-seeded values.
1. (Optional) You can setup redis but it's not required for development.
You can get the services up and running via the docker-compose.yaml.

#### Configuration notes
1. When setting up the Azure well-known/openid-configuration endpoint make sure it is prefixed with v2.0 `https://login.microsoftonline.com/tenant-id/v2.0/.well-known/openid-configuration`
1. When running with NODE_ENV=production all the requests made towards http will be redirected to https. This is intended to be used via secured proxy such as Nginx. So if you want to run with production flag, you need to make sure the service is exposed to https. You can check the code in `app/app.js Line 55` file.

#### How run locally (No Docker)
1. Copy .env-example into .env
1. Update the values for the configurations.
1. Copy file `app/support/example-oidc-client-apps.config.json` to `app/support/oidc-client-apps.config.json`
1. Update the file `app/support/oidc-client-apps.config.json` with the respective values for the clients you will be using it with. E.g. if you are running locally the Main Portal Angular frontend project on port 4201 you will need to add.
```
  {
    "client_id": "neispuo-portal",
    "response_types": ["code"],
    "redirect_uris": [
      "http://localhost:4201/signin-callback",
      "http://localhost:4201/silent-signin-callback"
    ],
    "post_logout_redirect_uris": ["http://localhost:4201/login"],
    "token_endpoint_auth_method": "none"
  }
```
#### How run in Docker
1. Copy .env-example-docker into .env
1. Update the values for in the Dockerfile.
1. Copy file `app/support/example-oidc-client-apps.config.json` to `app/support/oidc-client-apps.config.json`
1. Update the file `app/support/oidc-client-apps.config.json` with the respective values for the clients you will be using it with. E.g. if you are running locally the Main Portal Angular frontend project on port 4201 you will need to add.
```
  {
    "client_id": "neispuo-portal",
    "response_types": ["code"],
    "redirect_uris": [
      "http://localhost:4201/signin-callback",
      "http://localhost:4201/silent-signin-callback"
    ],
    "post_logout_redirect_uris": ["http://localhost:4201/login"],
    "token_endpoint_auth_method": "none"
  }
```
1. To build image locally run `docker build --no-cache -t mon-neispuo-openid-connect:1 .`
1. In order to run the container `docker run -d -p 3000:3000 -p 9229:9229 mon-neispuo-openid-connect:1`. Note the 9229 port is enabled for remote debugging purpose and can be omitted.

### Most relevant files

---

#### /app/app.js

This is the main entry file. It bootstraps the `Provider`, sets production flags and starts listening on the specified port.

The most notable changes made in this file are probably lines 37 and 38:

1. Line 37 (`provider.Session.prototype.promptedScopesFor`):
   1. This is a hacky community solution found in the github issues. It makes the OIDC skip the consent screen.
   2. The other non-hack solution provided by the author (can be found under `recipes`) was much more scattered and had a huge warning that he does not care or provide any support for the use case of removing consent screen.
   3. TLDR;
      1. This is the cleanest way to bypass consent screen, since 'consentless' is not supported by the framework.
      2. If this OIDC were to require consent flows in the future (eg. integration with third parties), this line will need to be removed or adapted according to the new requirements.
2. Line 38 (`provider.Session.prototype.getSelectedRole`):
   1. This line is here because role selection is not part of OIDC spec, and therefore is not a feature provided out of the box.
   2. In order to be able to issue a JWT containing `selected_role`, `Account.findAccount` is called by the framework, and it must know if and which role was selected.
      > Watch out: there is extra complexity here, since `Account.findAccount` is called in many different contexts and some of those contexts do not load the same data as others (eg pkce)

---

#### /app/routes/express.js

A lot of the app's logic is here. It is strongly recommended to visit the frameworks docs and learn about their `provider`, `session` and etc. There is a lot to know about, and it is out of the scope of this README.

Apart from the above, this file also contains all the new custom routes such as `forgot-password` and `azure-integration-callback`

---

#### /app/views/\*

This folder contains most of the view that the application will render using the `.ejs` templating engine.

Apart from the `*.ejs` files, `/views` also contains 3 subfolders:

1. `/js`: All individual js files that are consumed by `*.ejs` files go here
2. `/scss`: All custom stylesheets go here
3. `/dist`: Files from `/js` and `/scss` are processed by gulp tasks (`/gulpfile.js`) and dumped into this `dist` folder. This folder is then served by express as static assets (`app.js:25`)

---

#### /app/support/account.js

This is a pre-existing file that was adapted in several places.

Its main responsibility is to encapsulate the account logic (retrieving, validating/changing passwords etc), but by default the framework also uses it for generating token claims.

> Don't forget that `account` instances are not only used by our code, but by the framework itself as well! (eg. the framework expects that `account` instances will always have certain methods)

> Not everything related to claims are in this file. configuration.claims (under `configuration.js`) also affects what gets put into token claims or userInfo responses. See the framework docs for more.

---

#### /app/support/configuration.js

Most of the OIDC's behavior and functionality is configured here. For example:

1. Token expirations
1. Default views
   > May be those can be moved into `*.html` and `readFileSync`ed into the app. Their documentation says these must be string, but it is a bit strange to have html in js.
1. Enabled features (eg. sessionManagement)
1. Keys (jwt and cookie keys)
1. Interaction policies


#### /app/support/example-oidc-client-apps.config.json
1. The file displays the shape of the objects that need to be passed to the clients. Before running the project make sure `example-oidc-client-apps.config.json` is copied over to `oidc-client-apps.config.json` and set the respective clients for the particular environment. The file is being read at the start of the application in the `configuration.js` file. The reason for copying the file is so that we don't commit custom configurations and pollute commit history. 
1. Define the client(s) configurations in an array. This requires you to place JSON objects into the array which may vary depending on the deployment environment. Object shape looks like: 
```
{
    "client_id": "app-id",
    "response_types": ["code"],
    "redirect_uris": [
      "https://app.domain/signin-callback",
      "https://app.domain/silent-signin-callback",
      "http://app.domain/signin-callback",
      "http://app.domain/silent-signin-callback"
    ],
    "post_logout_redirect_uris": ["https://app.domain/login"],
    "token_endpoint_auth_method": "none"
  }
```

Most of the OIDC's behavior and functionality is configured here. For example:

1. Token expirations
1. Default views
   > May be those can be moved into `*.html` and `readFileSync`ed into the app. Their documentation says these must be string, but it is a bit strange to have html in js.
1. Enabled features (eg. sessionManagement)
1. Keys (jwt and cookie keys)
1. Interaction policies
---

#### /app/support/cookie-keys-dev.js

This file contains the keys used for cookie security. Copy paste it into `/support/cookie-keys-prod.js` and replace the exported array with an array containing 3 newly generated uuid strings.

---

#### /app/support/jwks-dev.js

This file contains the keys used for jwt security.

1. Copy paste `jwks-dev.js` into `jwks-prod.js`
2. Uncomment the `console.log`
3. Run the file with `node jwks-prod.js` and copy the output. Note you can run this if you navigate in the respective directory or from the project directory providing the full path to the script`node app/support/jwks-prod.js`.
4. In `jwks-prod.js`, paste the copied output: `module.exports = <<copied output>>`
5. Comment out the console.log to reduce future logs pollution

Note that different jwks-prod files will be committed in separate repositories depending on the deployment environments.

---

#### /app/support/selectRole.js

Custom interaction policy created to prompt users to select their roles. See the frameworks documentation for more.

---

#### /app/integrations/\*

Small modules that handle interacting with Azure's OIDC and Google's recaptcha.

---

#### /app/adapters/redis.js

OIDC needs persistance in order to track session and interaction states. There are several options, and we chose to use Redis. `redis.js` is their provided redis adapter, with very small modifications.

See their docs or example folder for more information about the other possible adapters and example code.

---

#### /import-accounts folder

This folder contains helper scripts that were used for generating SQL statements to create large amounts of users during presentation weeks.

Under `/src` you must look into each of the scripts (currently there are 4), and see if any of them can help in your use case. Some of them may not be useful as they require specific `.xlsx` files, but some of them may be helpful as they work with simple javascript objects.



#### How to build the image
1. Copy `.env-docker-example` to `.env`.
1. Copy `example-oidc-client-apps.config.json` to `oidc-client-apps.config.json` and update the development values.
1. Go to the Dockerfile directory and update the `Dockerfile` to reflect any values you might need modified.
1. Run `docker build --no-cache -t oidc:latest .` 

    - `-t` flag means tag if no tag is applied latest will be put in place.
    - `oidc` is the name of the image
    - `latest` is the tag of the image. It can be omitted or change do something like `1,2,stable`.

1. Verify docker image via `docker image ls`.
1. Run the container with command  `docker run -d -p 3000:3000 -p 9229:9229 oidc:latest` 
    - Port `3000` - the port to which the app will listen.
    - Port `9229` - is the port you can remotely debug on during dev mode.


#### How to push to shared Azure Repo from local image
1. Make sure you have the oidc image build locally as described in previous steps.
1. Install Azure CLI locally.
1. Login with the provided credentials for the ACR Repository
1. List directories via `az acr repository list --name neispuoprod`
1. Tag the image `docker tag oidc:latest neispuoprod.azurecr.io/oidc`
1. Push the image via `docker push neispuoprod.azurecr.io/oidc`
1. List directories via `az acr repository list --name neispuoprod` make sure the tag/or metadata is updated. You can use the following commands:
    ```
      az acr repository show-tags --name neispuoprod --repository oidc --output table
      asz acr repository show-manifests --name neispuoprod --repository oidc
    ```

