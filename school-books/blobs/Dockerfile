# syntax=docker/dockerfile:1

ARG VERSION=7.0-alpine

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build-env
WORKDIR /app

# Copy only the project files
# then restore packages as a distinct layer
COPY ./src/global.json ./src/SB.Blobs.sln ./src/SB.Blobs.csproj ./
RUN dotnet restore

# Copy everything else and build/publish
COPY ./src ./

# Define arguments for this build step as FROM resets all arguments
# https://docs.docker.com/engine/reference/builder/#understand-how-arg-and-from-interact
ARG APP_VERSION="1.0.0"

RUN dotnet build /p:TreatWarningsAsErrors=true /warnaserror -c Release --no-restore --version-suffix=$APP_VERSION
RUN dotnet publish "./SB.Blobs.csproj" -c Release --no-build --no-restore -o "./build"

###############################################################################
# Build runtime image for SB.Blobs
###############################################################################
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS blobs

# Add international components for unicode libraries (icu-libs) which is required
# to be able to disable .Net Globalization Invariant Mode as SqlClient requires it
RUN apk add --no-cache icu-libs

WORKDIR /app
COPY --from=build-env /app/build .

# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0
# reduce the number of opened files to prevent reaching maximum number of inotify instances
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
# disable .Net Globalization Invariant Mode as SqlClient doesn't work with it
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
# expose app(80), health(9101) and metrics(9102) ports
ENV ASPNETCORE_URLS="http://+:80;http://+:9101;http://+:9102"

ENTRYPOINT ["dotnet", "SB.Blobs.dll"]
