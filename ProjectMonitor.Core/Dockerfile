# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore first, so dependency layers are cached between builds
COPY ["ProjectMonitor.Core/ProjectMonitor.Core.csproj", "ProjectMonitor.Core/"]
COPY ["ProjectMonitor.Domain/ProjectMonitor.Domain.csproj", "ProjectMonitor.Domain/"]
RUN dotnet restore "ProjectMonitor.Core/ProjectMonitor.Core.csproj"

COPY . .
WORKDIR "/src/ProjectMonitor.Core"
RUN dotnet publish "ProjectMonitor.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProjectMonitor.Core.dll"]
