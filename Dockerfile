# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official ASP.NET Core SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["TravelDesk.csproj", "./"]
RUN dotnet restore "TravelDesk.csproj"

# Copy the rest of the source code
COPY . .
RUN dotnet build "TravelDesk.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TravelDesk.csproj" -c Release -o /app/publish

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for Render.io
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 8080
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "TravelDesk.dll"] 