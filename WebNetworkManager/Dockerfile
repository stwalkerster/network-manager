FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as build
ARG BuildId=0
ARG GitRev=devel
WORKDIR /opt
COPY . ./
RUN sed -i -re "s/v0 \\(devel\\)/v${BuildId} (${GitRev})/" "Views/Shared/_Layout.cshtml"
RUN ["dotnet", "publish", "WebNetworkManager.csproj"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /opt
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS="http://+:8080" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build /opt/bin/Debug/net5.0/publish/ ./
RUN apk add curl && chmod +x docker-entrypoint.sh

HEALTHCHECK CMD curl --insecure -Lfs http://localhost:8080/api/healthcheck || exit 1

ENTRYPOINT ["/opt/docker-entrypoint.sh"]