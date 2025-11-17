# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Gemona.sln ./

# Copy all project files
COPY Gemona.API/Gemona.API.csproj Gemona.API/
COPY Gemona.Application/Gemona.Application.csproj Gemona.Application/
COPY Gemona.Domain/Gemona.Domain.csproj Gemona.Domain/
COPY Gemona.Infrastructure/Gemona.Infrastructure.csproj Gemona.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy remaining source code
COPY . .

# Build the API project
WORKDIR /src/Gemona.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published files
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "Gemona.API.dll"]
