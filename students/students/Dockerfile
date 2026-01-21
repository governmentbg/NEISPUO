FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
#COPY src/*.csproj ./src
#COPY tests/*.csproj ./tests
COPY src/MON.API/*.csproj ./src/MON.API/
COPY src/MON.DataAccess/*.csproj ./src/MON.DataAccess/
COPY src/MON.ASPDataAcess/*.csproj ./src/MON.ASPDataAcess/
COPY src/MON.Models/*.csproj ./src/MON.Models/
COPY src/MON.Services/*.csproj ./src/MON.Services/
COPY src/MON.Shared/*.csproj ./src/MON.Shared/
COPY src/MON.BackgroundWorker/*.csproj ./src/MON.BackgroundWorker/
COPY tests/MON.UnitTests/*.csproj ./tests/MON.UnitTests/
COPY src/Report/MON.Report.Model/*.csproj ./src/Report/MON.Report.Model/
COPY src/Report/MON.Report.Service/*.csproj ./src/Report/MON.Report.Service/
COPY src/Regix/Kontrax.RegiX.Core/*.csproj ./src/Regix/Kontrax.RegiX.Core/
COPY src/Regix/Kontrax.RegiX.Standard.Client/*.csproj ./src/Regix/Kontrax.RegiX.Standard.Client/
COPY src/Regix/Kontrax.Utility.Certificate/*.csproj ./src/Regix/Kontrax.Utility.Certificate/
COPY src/Regix/Kontrax.Utility.Xsd/*.csproj ./src/Regix/Kontrax.Utility.Xsd/
COPY src/Templates/Domain/Domain.csproj ./src/Templates/Domain/

RUN dotnet restore

# copy everything else and build app
#COPY src/* ./src/*
#COPY tests/* ./tests/*
COPY src/MON.API/. ./src/MON.API/
COPY src/MON.DataAccess/. ./src/MON.DataAccess/
COPY src/MON.ASPDataAcess/. ./src/MON.ASPDataAcess/
COPY src/MON.Models/. ./src/MON.Models/
COPY src/MON.Services/. ./src/MON.Services/
COPY src/MON.Shared/. ./src/MON.Shared/
COPY src/MON.BackgroundWorker/. ./src/MON.BackgroundWorker/
COPY tests/MON.UnitTests/. ./tests/MON.UnitTests/
COPY src/Report/MON.Report.Model/. ./src/Report/MON.Report.Model/
COPY src/Report/MON.Report.Service/. ./src/Report/MON.Report.Service/
COPY src/Regix/Kontrax.RegiX.Core/. ./src/Regix/Kontrax.RegiX.Core/
COPY src/Regix/Kontrax.RegiX.Standard.Client/. ./src/Regix/Kontrax.RegiX.Standard.Client/
COPY src/Regix/Kontrax.Utility.Certificate/. ./src/Regix/Kontrax.Utility.Certificate/
COPY src/Regix/Kontrax.Utility.Xsd/. ./src/Regix/Kontrax.Utility.Xsd/
COPY src/Templates/Domain/. ./src/Templates/Domain/
COPY Lib/. ./Lib/

WORKDIR /app/src/MON.API
RUN dotnet publish -c Release -o out

###############################################################################
# Build runtime image for MON.API
############################################################################### 
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.25-focal
ARG VERSION
ARG DEBUG

# https://docs.telerik.com/reporting/use-reports-in-net-core-apps?&_ga=2.178811757.813670732.1615878129-696881724.1600854714#containerize-telerik-reporting
RUN apt-get update \ 
    && apt-get install -y --allow-unauthenticated \ 
        libc6-dev \ 
        libgdiplus \ 
        libx11-dev 

#Install on Ubuntu 20.04-focal latest version of gdiplus
#RUN apt-get update && apt-get install -y gnupg \
#	&& apt install -y gnupg ca-certificates \
#	&& apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
#	&& echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" | tee /etc/apt/sources.list.d/mono-official-stable.list \
#	&& apt update \
#	&& apt install -y libgdiplus

# Install ms fonts
#RUN apt-get install ttf-mscorefonts-installer -y && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build-env /app/src/MON.API/out .

# lower OpenSSL security settings in order to connect to regix-test
COPY openssl.conf /mnt/openssl.conf
ENV OPENSSL_CONF=/mnt/openssl.conf
ENV VERSION=$VERSION
# opt-out of the dotnet diagnostic pipeline to allow the container to run as readonly
ENV DOTNET_EnableDiagnostics=0

ENV DEBUG=$DEBUG

COPY src/Regix/certificates/regix.mon.bg.test.pfx /mnt/regix.mon.bg.test.pfx
COPY src/Regix/certificates/regix.mon.bg.pfx /mnt/regix.mon.bg.pfx

# disable reload on config
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=true 
ENV ASPNETCORE_URLS=http://+:80;http://+:9102
ENTRYPOINT ["dotnet", "MON.API.dll"]
EXPOSE 80 9102