FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine as build
WORKDIR /opt
COPY . ./
RUN ["dotnet", "publish", "WebNetworkManager.csproj"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /opt
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS="http://+:8080" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build /opt/bin/Debug/netcoreapp3.1/publish/ ./
RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ["/opt/docker-entrypoint.sh"]