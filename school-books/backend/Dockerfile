# syntax=docker/dockerfile:1

ARG VERSION=7.0-bullseye-slim

# using @rjgotten's solution from https://github.com/moby/moby/issues/15858#issuecomment-614157331

# Prefiltering stage using find -exec and cp --parents to copy out
# the project files in their proper directory structure.
FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build-env-prep
COPY . ./src/
RUN mkdir ./proj && cd ./src && \
  find . -type f -a \( -iname "global.json" -o -iname "*.sln" -o -iname "*.csproj" -o -iname "*.nupkg" \) \
    -exec cp --parents "{}" ../proj/ \;

# New build stage, independent cache
FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build-env
WORKDIR /app

# Copy only the project files with correct directory structure
# then restore packages as a distinct layer
COPY --from=build-env-prep ./proj ./
RUN dotnet restore

# Copy everything else and build/test/publish
COPY --from=build-env-prep ./src ./

# Define arguments for this build step as FROM resets all arguments
# https://docs.docker.com/engine/reference/builder/#understand-how-arg-and-from-interact
ARG APP_VERSION="1.0.0"

RUN dotnet build /warnaserror -c Release --no-restore --version-suffix=$APP_VERSION
RUN dotnet publish "./SB.Api/SB.Api.csproj" -c Release --no-build --no-restore -o "./build/api"
RUN dotnet publish "./SB.ExtApi/SB.ExtApi.csproj" -c Release --no-build --no-restore -o "./build/extapi"
RUN dotnet publish "./SB.JobHost/SB.JobHost.csproj" -c Release --no-build --no-restore -o "./build/jobs"
RUN dotnet publish "./SB.Cli/SB.Cli.csproj" -c Release --no-build --no-restore -o "./build/cli"

FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS tools

ARG DOTNET_TOOLS_VERSION=9.0.607501

# Install dotnet debug tools
RUN dotnet tool install --tool-path /tools dotnet-trace --version $DOTNET_TOOLS_VERSION \
 && dotnet tool install --tool-path /tools dotnet-counters --version $DOTNET_TOOLS_VERSION \
 && dotnet tool install --tool-path /tools dotnet-dump --version $DOTNET_TOOLS_VERSION \
 && dotnet tool install --tool-path /tools dotnet-gcdump --version $DOTNET_TOOLS_VERSION

###############################################################################
# Build runtime image for SB.Api
###############################################################################
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS api

# Add locale package
RUN apt update && apt install -y \
  locales \
  && rm -rf /var/lib/apt/lists/*

# Add bg_BG locale to the list of supported locales
RUN localedef -i bg_BG -c -f UTF-8 -A /usr/share/locale/locale.alias bg_BG.UTF-8

# Set timezone and locale
ENV \
  TZ=Europe/Sofia \
  LANG=bg_BG.UTF-8

# Setup root/intermediate certificates
# Note! The certificates must be in PEM format and with .crt extension
COPY ./certificates/* /usr/local/share/ca-certificates
RUN update-ca-certificates

# Copy dotnet-tools
COPY --from=tools /tools /tools

WORKDIR /app
COPY --from=build-env /app/build/api .

# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0
# reduce the number of opened files to prevent reaching maximum number of inotify instances
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
# expose app(80), health(9101) and metrics(9102) ports
ENV ASPNETCORE_URLS="http://+:80;http://+:9101;http://+:9102"

ENTRYPOINT ["dotnet", "SB.Api.dll"]

###############################################################################
# Build runtime image for SB.ExtApi
###############################################################################
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS extapi

# Add locale package
RUN apt update && apt install -y \
  locales \
  && rm -rf /var/lib/apt/lists/*

# Add bg_BG locale to the list of supported locales
RUN localedef -i bg_BG -c -f UTF-8 -A /usr/share/locale/locale.alias bg_BG.UTF-8

# Set timezone and locale
ENV \
  TZ=Europe/Sofia \
  LANG=bg_BG.UTF-8

# Setup root/intermediate certificates
# Note! The certificates must be in PEM format and with .crt extension
COPY ./certificates/* /usr/local/share/ca-certificates
RUN update-ca-certificates

# Copy dotnet-tools
COPY --from=tools /tools /tools

WORKDIR /app
COPY --from=build-env /app/build/extapi .

# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0
# reduce the number of opened files to prevent reaching maximum number of inotify instances
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
# expose app(80), health(9101) and metrics(9102) ports
ENV ASPNETCORE_URLS="http://+:80;http://+:9101;http://+:9102"

ENTRYPOINT ["dotnet", "SB.ExtApi.dll"]

###############################################################################
# Build runtime image for SB.JobHost
###############################################################################
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS jobs

# Add locale package and wkhtmltopdf dependancies
RUN apt update && apt install -y \
  locales \
  fontconfig libjpeg62-turbo libssl-dev libxext6 libxrender-dev xfonts-base xfonts-75dpi \
  && rm -rf /var/lib/apt/lists/*

# Add bg_BG locale to the list of supported locales
RUN localedef -i bg_BG -c -f UTF-8 -A /usr/share/locale/locale.alias bg_BG.UTF-8

# Set timezone and locale
ENV \
  TZ=Europe/Sofia \
  LANG=bg_BG.UTF-8

# Copy wkhtmltopdf
COPY --from=neispuoprod.azurecr.io/debian-wkhtmltopdf:0.12.6-bullseye-slim /usr/local/bin/wkhtmltopdf /bin/wkhtmltopdf

# Setup root/intermediate certificates
# Note! The certificates must be in PEM format and with .crt extension
COPY ./certificates/* /usr/local/share/ca-certificates
RUN update-ca-certificates

# Copy dotnet-tools
COPY --from=tools /tools /tools

WORKDIR /app
COPY --from=build-env /app/build/jobs .

# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0
# reduce the number of opened files to prevent reaching maximum number of inotify instances
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

# disable server GC and use non-concurrent GC
ENV DOTNET_gcServer=0
ENV DOTNET_gcConcurrent=0

# expose app(80), health(9101) and metrics(9102) ports
ENV ASPNETCORE_URLS="http://+:80;http://+:9101;http://+:9102"

ENTRYPOINT ["dotnet", "SB.JobHost.dll"]

###############################################################################
# Build runtime image for SB.Cli
###############################################################################
FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS cli

# Add locale package
RUN apt update && apt install -y \
  locales \
  && rm -rf /var/lib/apt/lists/*

# Add bg_BG locale to the list of supported locales
RUN localedef -i bg_BG -c -f UTF-8 -A /usr/share/locale/locale.alias bg_BG.UTF-8

# Set timezone and locale
ENV \
  TZ=Europe/Sofia \
  LANG=bg_BG.UTF-8

# Copy dotnet-tools
COPY --from=tools /tools /tools

WORKDIR /app
COPY --from=build-env /app/build/cli .

ENTRYPOINT ["dotnet", "SB.Cli.dll"]
