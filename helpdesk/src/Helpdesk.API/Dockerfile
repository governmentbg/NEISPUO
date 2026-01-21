#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Helpdesk.API/Helpdesk.API.csproj", "Helpdesk.API/"]
RUN dotnet restore "Helpdesk.API/Helpdesk.API.csproj"
COPY . .
WORKDIR "/src/Helpdesk.API"
RUN dotnet build "Helpdesk.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Helpdesk.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# disable reload on config
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=true 
ENTRYPOINT ["dotnet", "Helpdesk.API.dll"]
