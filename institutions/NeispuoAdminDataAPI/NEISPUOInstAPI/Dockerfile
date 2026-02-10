# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal


WORKDIR /app
COPY ./ /app

ENTRYPOINT ["dotnet", "NEISPUORegInstAPI.dll"]