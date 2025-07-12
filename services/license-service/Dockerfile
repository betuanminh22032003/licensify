# Dockerfile for license-service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "license-service.dll"]
