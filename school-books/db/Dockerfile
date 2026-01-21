# syntax=docker/dockerfile:1
# check=skip=SecretsUsedInArgOrEnv

ARG VERSION=2019-latest

# Build stage
FROM mcr.microsoft.com/mssql/server:$VERSION AS build-env
WORKDIR /app

ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=ContainerBuildPass1
ENV dbName=neispuo
ENV dbEnv=Dev

# Copy everything else and build database
COPY ./ ./
RUN ( /opt/mssql/bin/sqlservr & ) | grep -q "Service Broker manager has started" && \
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -No -i CreateAll.sql && \
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -No -Q "SHUTDOWN" | grep -q "Server shut down by request from login sa"

# Build runtime image
FROM mcr.microsoft.com/mssql/server:$VERSION AS db
WORKDIR /appdata

# Copy data files
COPY --from=build-env /var/opt/mssql/data/neispuo.mdf ./

# Copy attach db scripts
COPY ./docker-attach-db.sql /
COPY --chmod=775 ./docker-attach-db.sh /

ENV MSSQL_SA_PASSWORD=ContainerRunPass1
ENV dbName=neispuo

CMD ["/bin/bash", "-c", "/docker-attach-db.sh & /opt/mssql/bin/sqlservr"]
