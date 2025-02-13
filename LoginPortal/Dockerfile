# Base image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and projects
COPY ./LoginPortal.sln ./
COPY ./LoginPortal/LoginPortal.csproj ./LoginPortal/
COPY ./LoginPortalApp/LoginPortalApp.csproj ./LoginPortalApp/
COPY ./LoginPortal.Tests/LoginPortal.Tests.csproj ./LoginPortal.Tests/

# Restore dependencies
RUN dotnet restore

# Copy remaining files and build the app
COPY . .

# Publish LoginPortal API
WORKDIR /src/LoginPortal
RUN dotnet publish -c Release -o /app/publish

# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port and run the app
EXPOSE 80
ENTRYPOINT ["dotnet", "LoginPortal.dll"]
