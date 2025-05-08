# TodoList API Application

This is a .NET 8 Web API application for a Todo List, using MongoDB as a database.

## Deployment Instructions for Render

Follow these steps to fix deployment issues and successfully deploy the application on Render:

### 1. Required Environment Variables

Make sure these environment variables are set in your Render Dashboard:

- `MONGO_CONNECTION_STRING`: Your MongoDB connection string (e.g., `mongodb+srv://user:password@cluster.mongodb.net/?retryWrites=true&w=majority`)
- `JWT_SECRET`: Secret key for JWT token generation
- `ASPNETCORE_ENVIRONMENT`: Set to `Production`

The `PORT` variable is automatically set by Render.

### 2. Fix Deployment Issues

The following issues have been fixed in this repository:

1. **MongoDB Driver Version**: Updated to a stable version (2.22.0)
2. **SSL Configuration**: Properly configured SSL for MongoDB connections
3. **Docker Configuration**: Updated the Dockerfile with proper SSL certificate handling
4. **Kestrel Configuration**: Configured to listen on the correct port (from environment variable)
5. **Health Check Endpoint**: Added `/health` and `/ping` endpoints for monitoring
6. **Fixed Code Issues**: Resolved all build errors and missing model classes

### 3. Deployment with render.yaml

The repository includes a `render.yaml` file for easy deployment. Simply:

1. Fork this repository
2. Update the `repo` URL in `render.yaml` to point to your forked repository
3. Connect your GitHub account to Render
4. Create a new blueprint from your repository
5. Click "Apply Blueprint"

### 4. Manual Deployment Steps

If you prefer to deploy manually:

1. Clone the repository
2. Build the Docker image:
   ```
   docker build -t todolist-api -f ToDoList_FS/Dockerfile .
   ```
3. Set the required environment variables in Render
4. Deploy as a Web Service on Render, pointing to your repository

### 5. Troubleshooting

If you encounter "exit status 1" errors in Render:
- Check the build logs for specific errors
- Verify MongoDB connection settings
- Ensure all required environment variables are set
- Check SSL/TLS certificate issues

## Local Development

To run the application locally:

1. Clone the repository
2. Run `dotnet restore`
3. Run `dotnet build`
4. Run `dotnet run --project ToDoList_FS`

The API will be available at `http://localhost:8080` with Swagger UI at `http://localhost:8080/swagger` 