FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build-env
LABEL build-stage=intermediate
RUN apk add shadow \
    && useradd --no-log-init --create-home user \
    && mkdir /home/user/app \
    && chown user:user /home/user/app
USER user
WORKDIR /home/user/app
COPY --chown=user *.csproj ./
RUN dotnet restore
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
    ASPNETCORE_URLS="http://+:8080"
COPY --from=build-env --chown=user /home/user/app/out .
ENTRYPOINT ["dotnet", "WebNetworkManager.dll"]