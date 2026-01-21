## Installation

```bash
$ npm install
```

## Running the app

```bash
# development
$ npm run start

# watch mode
$ npm run start:dev

# production mode
$ npm run start:prod
```

## Test

```bash
# unit tests
$ npm run test

# e2e tests
$ npm run test:e2e

# test coverage
$ npm run test:cov
```

## To initialize database
```bash
$ npm run db:init
```

## To run all seeds
```bash
$ npm run db:seed
```

## To generate a migration after you changed an entity
```bash
$ npm run db:migration:generate
```


## Import DB from dev/test
1. Download back up file
1. Run docker-compose up -d
1. Copy the backup to the container volume: `docker cp ~/Downloads/neispuo_backup_2021_01_06_080002_2306969.bak mssql2019:/var/opt/mssql/data/backup2.bak`
1. Open Azure Data Studio - import db from the back up



# Docker build steps for MON Kubernetes Dev env

1. Update Dockerfile 
1. Run `docker build --no-cache -t rmi-backend .`
1. Run `docker tag rmi-backend:latest neispuoprod.azurecr.io/rmi-backend`
1. Run `docker push neispuoprod.azurecr.io/rmi-backend`
