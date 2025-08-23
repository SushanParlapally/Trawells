# Stage 1: Build Backend
# This stage compiles the C# application.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching
COPY TravelDesk.sln .
COPY TravelDesk.csproj .
COPY TravelDesk.Tests/TravelDesk.Tests.csproj ./TravelDesk.Tests/

# Restore NuGet packages for the entire solution
RUN dotnet restore "TravelDesk.sln"

# Copy the rest of the source code
COPY . .

# Publish the application, creating a release-optimized build
RUN dotnet publish "TravelDesk.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Final Backend Image
# This stage creates the final, lean image with only the compiled application and runtime.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the application will run on
EXPOSE 8080

# Define the command to run the application
ENTRYPOINT ["dotnet", "TravelDesk.dll"]
