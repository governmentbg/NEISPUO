FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Helpdesk.API/*.csproj ./Helpdesk.API/
COPY Helpdesk.DataAccess/*.csproj ./Helpdesk.DataAccess/
COPY Helpdesk.Models/*.csproj ./Helpdesk.Models/
COPY Helpdesk.Services/*.csproj ./Helpdesk.Services/
COPY Helpdesk.Shared/*.csproj ./Helpdesk.Shared/
COPY *.dcproj .

RUN dotnet restore

# copy everything else and build app
COPY Helpdesk.API/. ./Helpdesk.API/
COPY Helpdesk.DataAccess/. ./Helpdesk.DataAccess/
COPY Helpdesk.Models/. ./Helpdesk.Models/
COPY Helpdesk.Services/. ./Helpdesk.Services/
COPY Helpdesk.Shared/. ./Helpdesk.Shared/

WORKDIR /app/Helpdesk.API
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
ARG VERSION

WORKDIR /app
COPY --from=build-env /app/Helpdesk.API/out .

ENV VERSION=$VERSION
# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0
# disable reload on config
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=true 

ENTRYPOINT ["dotnet", "Helpdesk.API.dll"]