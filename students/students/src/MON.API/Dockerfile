#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/MON.API/MON.API.csproj", "src/MON.API/"]
COPY ["src/MON.Services/MON.Services.csproj", "src/MON.Services/"]
COPY ["src/MON.Shared/MON.Shared.csproj", "src/MON.Shared/"]
COPY ["src/MON.DataAccess/MON.DataAccess.csproj", "src/MON.DataAccess/"]
COPY ["src/MON.Models/MON.Models.csproj", "src/MON.Models/"]
COPY ["src/Regix/Kontrax.RegiX.Core/Kontrax.RegiX.Core.csproj", "src/Regix/Kontrax.RegiX.Core/"]
COPY ["src/Regix/Kontrax.Utility.Xsd/Kontrax.Utility.Xsd.csproj", "src/Regix/Kontrax.Utility.Xsd/"]
COPY ["src/Regix/Kontrax.Utility.Certificate/Kontrax.Utility.Certificate.csproj", "src/Regix/Kontrax.Utility.Certificate/"]
COPY ["src/Regix/Kontrax.RegiX.Standard.Client/Kontrax.RegiX.Standard.Client.csproj", "src/Regix/Kontrax.RegiX.Standard.Client/"]
COPY ["src/Report/MON.Report.Service/MON.Report.Service.csproj", "src/Report/MON.Report.Service/"]
COPY ["src/Report/MON.Report.Model/MON.Report.Model.csproj", "src/Report/MON.Report.Model/"]
RUN dotnet restore "src/MON.API/MON.API.csproj"
COPY . .
WORKDIR "/src/src/MON.API"
RUN dotnet build "MON.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MON.API.csproj" -c Release  /p:EnvironmentName=neispuo-production -o /app/publish

FROM base AS final
# https://docs.telerik.com/reporting/use-reports-in-net-core-apps?&_ga=2.178811757.813670732.1615878129-696881724.1600854714#containerize-telerik-reporting
RUN apt-get update \ 
    && apt-get install -y --allow-unauthenticated \ 
        libc6-dev \ 
        libgdiplus \ 
        libx11-dev \ 
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /mnt/certificate
COPY ["src/Regix/certificates/regix.mon.bg.test.pfx", "/mnt/certificate/"]
COPY ["src/Regix/certificates/regix.mon.bg.pfx", "/mnt/certificate/"]
ENTRYPOINT ["dotnet", "MON.API.dll"]