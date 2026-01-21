FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Diplomas.Public.API/*.csproj ./Diplomas.Public.API/
COPY Diplomas.Public.DataAccess/*.csproj ./Diplomas.Public.DataAccess/
COPY Diplomas.Public.Models/*.csproj ./Diplomas.Public.Models/
COPY Diplomas.Public.Services/*.csproj ./Diplomas.Public.Services/
COPY Diplomas.Public.Shared/*.csproj ./Diplomas.Public.Shared/

RUN dotnet restore

# copy everything else and build app
COPY Diplomas.Public.API/. ./Diplomas.Public.API/
COPY Diplomas.Public.DataAccess/. ./Diplomas.Public.DataAccess/
COPY Diplomas.Public.Models/. ./Diplomas.Public.Models/
COPY Diplomas.Public.Services/. ./Diplomas.Public.Services/
COPY Diplomas.Public.Shared/. ./Diplomas.Public.Shared/
#COPY Diplomas.Public.SPA/. ./Diplomas.Public.SPA/


WORKDIR /app/Diplomas.Public.API
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
ARG VERSION
ENV VERSION=$VERSION
# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0

WORKDIR /app
COPY --from=build-env /app/Diplomas.Public.API/out .

ENTRYPOINT ["dotnet", "Diplomas.Public.API.dll"]
EXPOSE 80 9102