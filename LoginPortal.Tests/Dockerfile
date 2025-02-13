# Use the official image for ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

# Use the SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file and project files
COPY ./LoginPortal.sln ./
COPY ./LoginPortal.Tests/LoginPortal.Tests.csproj ./LoginPortal.Tests/
COPY ./LoginPortalApp/LoginPortalApp.csproj ./LoginPortalApp/
COPY ./LoginPortal/LoginPortal.csproj ./LoginPortal/

# Restore dependencies
WORKDIR /src/LoginPortal.Tests
RUN dotnet restore

# Copy all source code
COPY ./LoginPortal/. ./LoginPortal/
COPY ./LoginPortalApp/. ./LoginPortalApp/
COPY ./LoginPortal.Tests/. ./LoginPortal.Tests/

# Build the test project
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish the test project
FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Running the tests
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entry point to run the tests
ENTRYPOINT ["dotnet", "test", "LoginPortal.Tests.dll"]
