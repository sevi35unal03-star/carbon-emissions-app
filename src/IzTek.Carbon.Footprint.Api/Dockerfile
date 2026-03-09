# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/IzTek.Carbon.Footprint.Api/IzTek.Carbon.Footprint.Api.csproj", "src/IzTek.Carbon.Footprint.Api/"]
COPY ["src/IzTek.Carbon.Footprint.Application/IzTek.Carbon.Footprint.Application.csproj", "src/IzTek.Carbon.Footprint.Application/"]
COPY ["src/IzTek.Carbon.Footprint.Domain/IzTek.Carbon.Footprint.Domain.csproj", "src/IzTek.Carbon.Footprint.Domain/"]
COPY ["src/IzTek.Carbon.Footprint.Infrastructure/IzTek.Carbon.Footprint.Infrastructure.csproj", "src/IzTek.Carbon.Footprint.Infrastructure/"]
COPY ["src/IzTek.Carbon.Footprint.Persistence/IzTek.Carbon.Footprint.Persistence.csproj", "src/IzTek.Carbon.Footprint.Persistence/"]
RUN dotnet restore "./src/IzTek.Carbon.Footprint.Api/IzTek.Carbon.Footprint.Api.csproj"
COPY . .
WORKDIR "/src/src/IzTek.Carbon.Footprint.Api"
RUN dotnet build "./IzTek.Carbon.Footprint.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IzTek.Carbon.Footprint.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IzTek.Carbon.Footprint.Api.dll"]