# Use the official ASP.NET Core runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file and project file, then restore dependencies
COPY ../LoginPortal.sln ./                
# Go up one level to access the solution file
COPY ./LoginPortalApp.csproj ./LoginPortalApp/  
# Copy project file from current folder


# Copy all source code
COPY . ./LoginPortalApp/                    
# Copy the rest of the source code

# Build the project
WORKDIR /src/LoginPortalApp
RUN dotnet build "LoginPortalApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the service
FROM build AS publish
RUN dotnet publish "LoginPortalApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Final stage - Production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the app on the required ports
EXPOSE 8080
EXPOSE 8081

# Entry point to run the API
ENTRYPOINT ["dotnet", "LoginPortalApp.dll"]
