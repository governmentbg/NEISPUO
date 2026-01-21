# OpenID Connect (OIDC) Server

## This POC includes:

1. App1 frontend with code flow and example backend jwks for validating tokens
2. App2 frontend only with code flow
3. OpenID Connect server with
    1. Local user logins
    1. Example Azure integration
    1. SSO using Redis store

## OIDC dev environment

> The previous `localhost-enabled` branch is deprecated. `git checkout master` and follow steps bellow.

1. `cp openid/openid-server/.env-example openid/openid-server/.env`
2. Fill the `.env` file:
    1. local or test DB credentials is mandatory
    2. make sure `NODE_ENV=development`
    3. Azure can be left blank if you are not going to use it (ignore azure startup errors in this case)
    4. Recaptcha can be left blank, it does not activate during development
3. Run OIDC with
    ```
    cd openid/openid-server
    npm install
    npm run start:dev
    ```
4. In dev mode, all sessions will be cleared on every OIDC restart.

## Demo frontend

1. If you want a demo frontend that consumes the OIDC
    ```
    cd app2
    npm install
    npm start
    ```
2. Navigate to localhost:4202 and click login
3. You should provide `sysUser` credentials from local or test MSSQL server (whichever you use in .env)
