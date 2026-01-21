#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Diplomas.Public.API/Diplomas.Public.API.csproj", "Diplomas.Public.API/"]
COPY ["Diplomas.Public.Models/Diplomas.Public.Models.csproj", "Diplomas.Public.Models/"]
COPY ["Diplomas.Public.Services/Diplomas.Public.Services.csproj", "Diplomas.Public.Services/"]
COPY ["Diplomas.Public.DataAccess/Diplomas.Public.DataAccess.csproj", "Diplomas.Public.DataAccess/"]
RUN dotnet restore "Diplomas.Public.API/Diplomas.Public.API.csproj"
COPY . .
WORKDIR "/src/Diplomas.Public.API"
RUN dotnet build "Diplomas.Public.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Diplomas.Public.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Diplomas.Public.API.dll"]