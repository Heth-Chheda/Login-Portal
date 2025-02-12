# Use the official image for ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY LoginPortal/LoginPortal.csproj LoginPortal/
RUN dotnet restore "LoginPortal/LoginPortal.csproj"

# Copy the rest of the source code
COPY LoginPortal/ LoginPortal/

# Build the project
WORKDIR /src/LoginPortal
RUN dotnet build "LoginPortal.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the service
FROM build AS publish
RUN dotnet publish "LoginPortal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entry point to run the API
ENTRYPOINT ["dotnet", "LoginPortal.dll"]
