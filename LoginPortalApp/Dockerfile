# Use the official image for ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-1809 AS build
WORKDIR /src

# Switch to root user temporarily for restore/build process
USER root

# Copy the solution and restore dependencies
COPY ["LoginPortal.sln", "./"]
COPY ["LoginPortalApp/LoginPortalApp.csproj", "LoginPortalApp/"]
COPY ["LoginPortal/LoginPortal.csproj", "LoginPortal/"]
RUN dotnet restore "LoginPortal.sln"

# Copy the rest of the code
COPY . .

# Build the project
WORKDIR "/src/LoginPortalApp"
RUN dotnet build "LoginPortalApp.csproj" -c Release -o /app/build

# Publish the service
FROM build AS publish
RUN dotnet publish "LoginPortalApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage - Production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the app
EXPOSE 8080
EXPOSE 8081

# Entry point to run the app
ENTRYPOINT ["dotnet", "LoginPortalApp.dll"]
