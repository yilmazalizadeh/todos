# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

COPY TravelApp.sln ./
COPY TravelApp/TravelApp.csproj TravelApp/
COPY TravelApp.Test/TravelApp.Test.csproj TravelApp.Test/
RUN dotnet restore TravelApp.sln

FROM restore AS test
COPY . .
RUN dotnet test TravelApp.sln --configuration Release --no-restore

FROM restore AS publish
COPY . .
RUN dotnet publish TravelApp/TravelApp.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl jq \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=publish /app/publish .
COPY scripts/docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh

RUN chmod +x /usr/local/bin/docker-entrypoint.sh

ENTRYPOINT ["docker-entrypoint.sh"]
CMD ["dotnet", "TravelApp.dll"]
