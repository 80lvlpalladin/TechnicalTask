# Use the official Microsoft .NET SDK image as the base image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy solution files
COPY PixelService/PixelService.sln ./PixelService/
COPY StorageService/StorageService.sln ./StorageService/

# Copy csproj files and related directories for PixelService
COPY PixelService/PixelService.API/PixelService.API.csproj ./PixelService/PixelService.API/
COPY PixelService/PixelService.Domain/PixelService.Domain.csproj ./PixelService/PixelService.Domain/
COPY PixelService/PixelService.Features/PixelService.Features.csproj ./PixelService/PixelService.Features/
COPY PixelService/PixelService.Infrastructure/PixelService.Infrastructure.csproj ./PixelService/PixelService.Infrastructure/

# Copy csproj files and related directories for StorageService
COPY StorageService/StorageService.Application/StorageService.Application.csproj ./StorageService/StorageService.Application/
COPY StorageService/StorageService.Domain/StorageService.Domain.csproj ./StorageService/StorageService.Domain/
COPY StorageService/StorageService.Features/StorageService.Features.csproj ./StorageService/StorageService.Features/
COPY StorageService/StorageService.Infrastructure/StorageService.Infrastructure.csproj ./StorageService/StorageService.Infrastructure/


# Restore dependencies
RUN dotnet restore PixelService/PixelService.sln
RUN dotnet restore StorageService/StorageService.sln

# Copy the rest of the source code
COPY PixelService/ ./PixelService/
COPY StorageService/ ./StorageService/

# Build the applications
RUN dotnet build PixelService/PixelService.sln -c Release -o /app/build
RUN dotnet build StorageService/StorageService.sln -c Release -o /app/build

# Run tests
RUN dotnet test PixelService/PixelService.Features.Tests/PixelService.Features.Tests.csproj
RUN dotnet test StorageService/StorageService.Infrastructure.Tests/StorageService.Infrastructure.Tests.csproj

# Publish the applications
RUN dotnet publish PixelService/PixelService.API/PixelService.API.csproj -c Release -o /app/publish/PixelService
RUN dotnet publish StorageService/StorageService.Application/StorageService.Application.csproj -c Release -o /app/publish/StorageService
