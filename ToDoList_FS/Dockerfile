# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

USER root
RUN apt-get update && apt-get install -y openssl && rm -rf /var/lib/apt/lists/*

USER app

EXPOSE 5000
EXPOSE 5001

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ToDoList_FS/ToDoList_FS.csproj", "ToDoList_FS/"]
RUN dotnet restore "./ToDoList_FS/ToDoList_FS.csproj"

COPY . .

WORKDIR "/src/ToDoList_FS"
RUN dotnet build "./ToDoList_FS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ToDoList_FS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV MONGO_TLS=true
ENV MONGO_SSL=true

# Start application
ENTRYPOINT ["dotnet", "ToDoList_FS.dll"]
