FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./OsuCzechiaBot", "/src"]
ARG TARGETARCH
RUN dotnet restore "OsuCzechiaBot.csproj" -a $TARGETARCH
RUN dotnet publish "./OsuCzechiaBot.csproj" -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OsuCzechiaBot.dll"]
