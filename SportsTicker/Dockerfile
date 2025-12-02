# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet@sha256:7c4246c1c384319346d45b3e24a10a21d5b6fc9b36a04790e1588148ff8055b0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ARG AUTHORS
ARG VERSION
ARG TITLE
ARG DESCRIPTION

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk@sha256:c7445f141c04f1a6b454181bd098dcfa606c61ba0bd213d0a702489e5bd4cd71 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["SportsTicker/SportsTicker.csproj", "SportsTicker/"]
RUN dotnet restore "./SportsTicker/SportsTicker.csproj"
COPY . .
WORKDIR "/src/SportsTicker"
RUN dotnet build "./SportsTicker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SportsTicker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

LABEL \
    org.opencontainers.image.authors="${AUTHORS}" \
    org.opencontainers.image.title="${TITLE}" \
    org.opencontainers.image.description="${DESCRIPTION}" \
    org.opencontainers.image.version="${VERSION}" \
    org.opencontainers.image.licenses="MIT"

ENTRYPOINT ["dotnet", "SportsTicker.dll"]
