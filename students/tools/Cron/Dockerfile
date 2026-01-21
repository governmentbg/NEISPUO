FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Neispuo.Tools/*.sln .
COPY Neispuo.Tools/Neispuo.Tools.Report/*.csproj ./Neispuo.Tools.Report/
COPY Neispuo.Tools/Neispuo.Tools.DataAccess/*.csproj ./Neispuo.Tools.DataAccess/
COPY Neispuo.Tools/Neispuo.Tools.Services/*.csproj ./Neispuo.Tools.Services/
COPY Neispuo.Tools/Neispuo.Tools.Shared/*.csproj ./Neispuo.Tools.Shared/
COPY Neispuo.Tools/Neispuo.Tools.Models/*.csproj ./Neispuo.Tools.Models/

RUN dotnet restore

# copy everything else and build app
COPY Neispuo.Tools/Neispuo.Tools.Report/. ./Neispuo.Tools.Report/
COPY Neispuo.Tools/Neispuo.Tools.DataAccess/. ./Neispuo.Tools.DataAccess/
COPY Neispuo.Tools/Neispuo.Tools.Services/. ./Neispuo.Tools.Services/
COPY Neispuo.Tools/Neispuo.Tools.Shared/. ./Neispuo.Tools.Shared/
COPY Neispuo.Tools/Neispuo.Tools.Models/. ./Neispuo.Tools.Models/

WORKDIR /app/Neispuo.Tools.Report/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy

# Install tzdata non-interactively
RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y tzdata && \
    ln -sf /usr/share/zoneinfo/Europe/Sofia /etc/localtime && \
    echo "Europe/Sofia" > /etc/timezone

RUN apt update && apt install cron -y

WORKDIR /app
COPY --from=build /app/Neispuo.Tools.Report/out ./Neispuo.Tools.Report

RUN apt-get update \ 
    && apt-get install -y  \ 
       dos2unix

# Add crontab file in the cron directory
ADD Cronfiles /etc/cron.d/
RUN dos2unix /etc/cron.d/*

# Add scripts
ADD Scripts /scripts
RUN dos2unix /scripts/*
 
# Give execution rights on the cron job
RUN chmod 0644 /etc/cron.d/*

# Give execution rights on the scripts
RUN chmod 0755 /scripts/*

COPY --chmod=775 ./docker-start.sh /
RUN dos2unix /docker-start.sh

WORKDIR /etc/cron.d

# Run the command on container startup
CMD ["/docker-start.sh"]
