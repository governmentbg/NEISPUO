# backend

### Project setup

0. Prerequisites

- nodejs active LTS or maintenance LTS version ([an Angular requirement](https://angular.io/guide/setup-local#prerequisites))
- dotnet v6
- docker (required for openapitools/openapi-generator-cli)
- git
- local sql server
  - On an intel mac use sqlserver docker image  
    `docker run --restart unless-stopped --name mssql-server -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=<sa pass>' -e 'TZ=Europe/Sofia' -p 127.0.0.1:1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest`
  - On an arm mac use azure-sql-edge docker image  
    `docker run --restart unless-stopped --name azuresqledge -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<sa pass>" -e 'TZ=Europe/Sofia' -p 127.0.0.1:1433:1433 -d mcr.microsoft.com/azure-sql-edge`
- local redis  
  `docker run --restart unless-stopped --name redis -p 127.0.0.1:6379:6379 -d redis`

1. Set the following environment variables. They are used both by the db build and the web app.

- `SB__Data__DbIP`
- `SB__Data__DbPort`
- `SB__Data__DbUser`
- `SB__Data__DbPass`
- `SB__Data__DbName`

2. Install ODBC sqlcmd (different from the new https://github.com/microsoft/go-sqlcmd)

- On Windows it is installed with the SQL Server Developer Edition
- On Macos use homebrew (check the [docs](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup-tools?view=sql-server-ver15#macos) for more details)

  ```sh
  # brew untap microsoft/mssql-preview if you installed the preview version
  brew tap microsoft/mssql-release https://github.com/Microsoft/homebrew-mssql-release
  brew update
  brew install mssql-tools
  #for silent install:
  #HOMEBREW_NO_ENV_FILTERING=1 ACCEPT_EULA=y brew install mssql-tools
  ```

  If you get the following error after update of homebrew on Apple Silicon (ARM):

  ```
  Sqlcmd: Error: Microsoft ODBC Driver 17 for SQL Server : SSL Provider: [OpenSSL library could not be loaded, make sure OpenSSL 1.0 or 1.1 is installed].
  Sqlcmd: Error: Microsoft ODBC Driver 17 for SQL Server : Client unable to establish connection.
  ```

  Run:

  ```sh
  rm /opt/homebrew/opt/openssl

  cd /opt/homebrew/opt
  ln -s ../Cellar/openssl@1.1/1.1.1s /opt/homebrew/opt/openssl
  ```

3. To sync with the NEISPUO Test deployment database

- Install Python 2 >=2.7.9 or Python 3 >=3.4 (we need [pip](https://pip.pypa.io/en/stable/installing/))
- Install `mssql-scripter` with `pip install mssql-scripter`
- Install `dos2unix` with `brew install dos2unix`. NOTE! this step is macos only, a windows alternative needs to be done.
- Set the following environment variables.
  - `SB__Data__DbIP_MON`
  - `SB__Data__DbPort_MON`
  - `SB__Data__DbUser_MON`
  - `SB__Data__DbPass_MON`
  - `SB__Data__DbName_MON`
- Run `npm run generate-other-schema-scripts`

5. Build the DB by running `npm run deploy` inside the `db` folder.

6. Run the SB.Api project.

7. Run the OIDC stub at `stubs/oidc-provider-stub` with `node index.js`

8. Continuously serve the Angular app by running `npm start` inside the `frontend` folder.

9. To run the html-template cli command

- Login to the neispuo container registry with (required only once)
  ```sh
  docker login neispuoprod.azurecr.io -u neispuoprod -p "<password>"
  ```
- Run `dotnet run html-template Book_CDO --pdf` in `backend/SB.Cli`

10. To run the tests (unit and integration)

- Make sure you have the follwoing BTrust certificates added to the machine (https://www.b-trust.bg/bg/queries/certification-chains-installation)
  - B-Trust Root Qualified CA
  - B-Trust Operational Qualified CA
- Start the blobs service without logging
  ```sh
  cd blobs/src
  export Serilog__MinimumLevel__Default="Fatal"
  export Serilog__MinimumLevel__Override__Microsoft="Fatal"
  export Serilog__MinimumLevel__Override__System="Fatal"
  dotnet run
  ```
- Run all tests
  ```sh
  dotnet test
  ```

11. To build and run the app with containers (as the CI/CD would do)

- Add `neispuo_school_books_ci_ed25519` and `neispuo_school_books_ci_ed25519.pub` SSH key files to `~/.ssh`
- Build and run the frontend

  ```sh
  docker build -t frontend \
      --build-arg SSH_KEY_PUB="$(cat ~/.ssh/neispuo_school_books_ci_ed25519.pub)" \
      --build-arg SSH_KEY_BASE64="$(cat ~/.ssh/neispuo_school_books_ci_ed25519 | base64)" \
      --target frontend .

  docker run -it --rm -p 4200:80 \
      --env SB__FRONTEND__API__BASE__PATH='http://localhost:5001' \
      --env SB__FRONTEND__AUTH__SERVER__PATH='http://localhost:3000' \
      --env SB__FRONTEND__AUTH__REQUIRE__HTTPS='FALSE' \
      --env SB__FRONTEND__PORTAL__USER_GUIDE='https://www.google.com' \
      --env SB__FRONTEND__BLOB__SERVER__PATH='http://localhost:5100' \
      --env SB__FRONTEND__SIGNING__SERVER__PATH='http://localhost:5339' \
      frontend:latest
  ```

- Build and run the backend

  ```sh
  docker build -t backend --target api .

  docker run -it --rm -p 5001:80 \
      --env ASPNETCORE_ENVIRONMENT='Development' \
      --env SB__Data__DbIP='host.docker.internal' \
      --env SB__Data__DbPort='1433' \
      --env SB__Data__DbUser='sa' \
      --env SB__Data__DbPass='pass' \
      --env SB__Data__DbName='neispuo' \
      --env SB__Domain__RedisConnectionString='host.docker.internal,abortConnect=false,connectTimeout=30000,responseTimeout=30000' \
      --env SB__Api__OIDCAuthority='http://host.docker.internal:3000' \
      backend:latest
  ```

- Build and run the extapi

  ```sh
  docker build -t extapi --target extapi .

  docker run -it --rm -p 3001:3001 \
      --env ASPNETCORE_ENVIRONMENT='Development' \
      --env SB__Data__DbIP='host.docker.internal' \
      --env SB__Data__DbPort='1433' \
      --env SB__Data__DbUser='sa' \
      --env SB__Data__DbPass='pass' \
      --env SB__Data__DbName='neispuo' \
      --env SB__Domain__RedisConnectionString='host.docker.internal,abortConnect=false,connectTimeout=30000,responseTimeout=30000' \
      --env SB__ExtApi__ForwardedHeadersKnownNetworks__0='172.17.0.1/24' \
      --env SB__Domain__BlobServicePublicUrl='http://host.docker.internal:5100' \
      --env SB__Domain__BlobServiceHMACKey='pBLzUmoEHTcoEva6UELH' \
      extapi:latest
  ```

- Build and run the jobs host

  ```sh
  docker build -t jobs --target jobs .

  docker run -it --rm \
      --env ASPNETCORE_ENVIRONMENT='Development' \
      --env SB__Data__DbIP='host.docker.internal' \
      --env SB__Data__DbPort='1433' \
      --env SB__Data__DbUser='sa' \
      --env SB__Data__DbPass='pass' \
      --env SB__Data__DbName='neispuo' \
      --env SB__Domain__RedisConnectionString='host.docker.internal,abortConnect=false,connectTimeout=30000,responseTimeout=30000' \
      --env SB__Domain__BlobServicePublicUrl='http://host.docker.internal:5100' \
      --env SB__Domain__BlobServiceHMACKey='pBLzUmoEHTcoEva6UELH' \
      jobs:latest
  ```

- If you need to work with files like generating and printing files, you need to build and run blobs container from blobs folder. Check blobs [README.md](../blobs/README.md).

- Build and run frontend-logger-sink

  ```sh
    docker build -t logger --build-arg APP_VERSION="123" --target logger .

    docker run -it --rm \
        --env ASPNETCORE_ENVIRONMENT='Development' \
        --env SB__Data__DbIP='host.docker.internal' \
        --env SB__Data__DbPort='1433' \
        --env SB__Data__DbUser='sa' \
        --env SB__Data__DbPass='pass' \
        --env SB__Data__DbName='neispuo' \
        --env SB__Logger__App='dnevnik-logger' \
        --env SB__Logger__Pod='dnevnik-logger-123456' \
        --env SB__Logger__SourcemapsUrl='http://127.0.0.1:4200' \
        logger:latest
  ```

12. Kubernets setup

- Install https://kubernetes.io/docs/tasks/tools/#kubectl
- Add those domains in the machine's hosts file
  ```
  10.200.200.70     k8-mon.local
  10.200.200.82     argocd.mon.bg
  ```
- Add `config` kubectl config file to `~/.kube`
- Connect to MON's VPN

  ```pwsh
  # Connect.ps1
  Write-Output '<password>' | &'C:\Program Files\OpenConnect\openconnect.exe' --protocol=anyconnect --server='https://vpngw.mon.bg' --user='<username with @mon.bg>' --passwd-on-stdin --non-inter
  ```

  ```sh
  #!/bin/bash
  # connect.sh
  echo "<password>" | sudo openconnect 'https://vpngw.mon.bg' --user='<username with @mon.bg>' --passwd-on-stdin --servercert pin-sha256:cdGjJKelvf+pUjmK9DiURymfpXeV2piERQXtqL/vhlU=
  ```

- Test with `kubectl cluster-info`

13. kubectl Cheat Sheat - https://kubernetes.io/docs/reference/kubectl/cheatsheet/

- `kubectl port-forward service/kube-prometheus-stack-grafana 9080:80 -n monitoring`
- `kubectl get pods -n test`
- `kubectl logs <pod-name> -n test`
- `kubectl describe pod <pod-name> -n test`
- `kubectl delete pod <pod-name> -n test`
- `kubectl get replicasets -n test`
- `kubectl delete replicaset <replicaset-name> -n test`
- `kubectl describe ingress test-ingress-integrations -n test`
- `kubectl replace -f test/ingress-integrations.yaml -n test`
- `kubectl exec -it -n test -c <container> <pod-name> -- /bin/bash`
- `kubectl rollout restart deployment dnevnik-extapi-test -n test`
- `kubectl scale deployment/dnevnik-extapi-prd --replicas=0 -n prd`
- Connect to redis
  - Get the password
    `kubectl get secret dnevn-redis-pass -n prd -ojson | jq -r '.data["password"]' | base64 -d`
  - Port forward
    `kubectl port-forward service/dnevn-redis-prd 6380:6379 -n prd`
  - Connect
    `redis-cli -p 6380 -a <password>`
- View the node resources
  - Install the `krew` kubectl package manager https://krew.sigs.k8s.io/docs/user-guide/setup/install/
  - Install the `resource-capacity` krew plugin `kubectl krew install resource-capacity`
  - Run `kubectl resource-capacity`
  - Check https://github.com/robscott/kube-capacity for more options
  - `kubectl get pods -n test | (head -n 1; grep -e "dnevnik-.*-test")`
  - `kubectl resource-capacity --pods --util | (head -n 1; grep -e "dnevnik-.*-test")`
- View all requests arriving at the ingress for a specific service
  `kubectl logs -f --tail=20 -n dev ingress-dev-ingress-nginx-controller-6ff48ddf7c-hfm42 | grep dev-blobs`
