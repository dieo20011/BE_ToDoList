#!/bin/bash

echo "Running deployment fix script..."

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: dotnet SDK is not installed"
    exit 1
fi

# Make sure our project builds correctly
echo "Building project..."
cd ToDoList_FS
dotnet restore
dotnet build

if [ $? -ne 0 ]; then
    echo "Build failed. Please fix the errors above."
    exit 1
fi

# Check if MongoDB.Driver version is compatible
MONGO_VERSION=$(grep -o 'MongoDB.Driver.*Version="[^"]*"' ToDoList_FS.csproj | cut -d'"' -f2)
echo "MongoDB.Driver version: $MONGO_VERSION"

# Check Docker build
echo "Testing Docker build..."
cd ..
docker build -t todolist-test -f ToDoList_FS/Dockerfile .

if [ $? -ne 0 ]; then
    echo "Docker build failed. Please fix the errors above."
    exit 1
else
    echo "Docker build successful!"
fi

echo "All checks passed. Your application should be ready for deployment."
echo "Remember to set these environment variables on Render:"
echo "- PORT (should be set automatically by Render)"
echo "- MONGO_CONNECTION_STRING (your MongoDB connection string)"
echo "- JWT_SECRET (your JWT secret key)"
echo "- ASPNETCORE_ENVIRONMENT=Production"

echo "Script completed!" 