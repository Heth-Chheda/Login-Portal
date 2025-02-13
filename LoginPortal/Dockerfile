# Base image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

<<<<<<< HEAD
# Copy solution and projects
COPY ./LoginPortal.sln ./
COPY ./LoginPortal/LoginPortal.csproj ./LoginPortal/
COPY ./LoginPortalApp/LoginPortalApp.csproj ./LoginPortalApp/
COPY ./LoginPortal.Tests/LoginPortal.Tests.csproj ./LoginPortal.Tests/

# Restore dependencies
RUN dotnet restore
=======
# Copy the solution file and project file, then restore dependencies
COPY ../LoginPortal.sln ./                
# Go up one level to access the solution file
COPY ./LoginPortal.csproj ./LoginPortal/  
# Copy project file from current folder
RUN dotnet restore "LoginPortal/LoginPortal.csproj"

# Copy all source code
COPY . ./LoginPortal/                    
# Copy the rest of the source code
>>>>>>> 6edac4fe61dcc281d2cf1816292c27b272d2e296

# Copy remaining files and build the app
COPY . .

# Publish LoginPortal API
WORKDIR /src/LoginPortal
RUN dotnet publish -c Release -o /app/publish

<<<<<<< HEAD
# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
=======
# Publish the service
FROM build AS publish
RUN dotnet publish "LoginPortal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
>>>>>>> 6edac4fe61dcc281d2cf1816292c27b272d2e296
WORKDIR /app
COPY --from=build /app/publish .

<<<<<<< HEAD
# Expose port and run the app
EXPOSE 80
=======
# Expose the app on the required ports
EXPOSE 8080
EXPOSE 8081

# Entry point to run the API
>>>>>>> 6edac4fe61dcc281d2cf1816292c27b272d2e296
ENTRYPOINT ["dotnet", "LoginPortal.dll"]
