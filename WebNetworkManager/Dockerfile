FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build-env
LABEL build-stage=intermediate
RUN apk add shadow \
    && useradd --no-log-init --create-home user \
    && mkdir /home/user/app \
    && chown user:user /home/user/app
USER user
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /home/user/app
COPY --chown=user *.csproj ./
RUN mkdir -p .nuget/NuGet \
    echo '<?xml version="1.0" encoding="utf-8"?><configuration><packageSources><add key="nexus" value="https://nexus.scimonshouse.net/repository/nuget-group/" /></packageSources></configuration>' > .nuget/NuGet/NuGet.Config \
    dotnet restore
COPY --chown=user . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine
MAINTAINER Simon Walker <simon@stwalkerster.co.uk>
LABEL build-stage=final
RUN apk add shadow \
    && useradd --no-log-init --create-home user \
    && mkdir /home/user/app \
    && chown user:user /home/user/app
USER user
WORKDIR /home/user/app
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS="http://+:8080" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY --from=build-env --chown=user /home/user/app/out .
ENTRYPOINT ["dotnet", "WebNetworkManager.dll"]