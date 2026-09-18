# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:6a94333d37514e385650a3c81a55e5350b67253dbe136e9cf17e499c35606a8c AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:2fa828c68761b1b8c23d7662dc134421b9d3b59fe1425fdbc80804e390cdb24d AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Examples/Directory.Build.props", "Examples/"]
COPY ["OSLC4Net.Server.Providers/OSLC4Net.Server.Providers.csproj", "OSLC4Net.Server.Providers/"]
COPY ["OSLC4Net.ChangeManagement/OSLC4Net.ChangeManagementCommon.csproj", "OSLC4Net.ChangeManagement/"]
COPY ["Examples/OSLC4NetExamples.Server.NetCoreApi/OSLC4NetExamples.Server.NetCoreApi.csproj", "Examples/OSLC4NetExamples.Server.NetCoreApi/"]
RUN dotnet restore "./Examples/OSLC4NetExamples.Server.NetCoreApi/OSLC4NetExamples.Server.NetCoreApi.csproj"
COPY . .
WORKDIR "/src/Examples/OSLC4NetExamples.Server.NetCoreApi"
RUN dotnet build "./OSLC4NetExamples.Server.NetCoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OSLC4NetExamples.Server.NetCoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OSLC4NetExamples.Server.NetCoreApi.dll"]
