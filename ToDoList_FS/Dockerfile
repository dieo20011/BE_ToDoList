# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER root
# Install certificates and other dependencies
RUN apt-get update && \
    apt-get install -y openssl ca-certificates && \
    update-ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# We'll use PORT environment variable from Render
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

# Copy csproj and restore dependencies
COPY ["ToDoList_FS/ToDoList_FS.csproj", "ToDoList_FS/"]
RUN dotnet restore "ToDoList_FS/ToDoList_FS.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/ToDoList_FS"
RUN dotnet build "ToDoList_FS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ToDoList_FS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV MONGO_TLS=true
ENV MONGO_SSL=true
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

# Start application
ENTRYPOINT ["dotnet", "ToDoList_FS.dll"]
