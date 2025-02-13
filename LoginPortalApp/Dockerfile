# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file
COPY ../LoginPortal.sln ./

# Copy project files
COPY ../LoginPortalApp/LoginPortalApp.csproj ./LoginPortalApp/
COPY ../LoginPortal/LoginPortal.csproj ./LoginPortal/

# Restore dependencies
RUN dotnet restore "LoginPortalApp/LoginPortalApp.csproj"

# Copy all source code for both projects
COPY ../LoginPortal/ ./LoginPortal/
COPY ../LoginPortalApp/ ./LoginPortalApp/

# Build the LoginPortalApp project
WORKDIR /src/LoginPortalApp
RUN dotnet build "LoginPortalApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "LoginPortalApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "LoginPortalApp.dll"]
