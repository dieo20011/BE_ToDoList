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

If you're experiencing deployment issues, check the following:

1. **MongoDB Driver Version**: Make sure you're using a stable version (2.22.0 is recommended).
2. **SSL Configuration**: Ensure SSL is properly configured for MongoDB connections.
3. **Docker Configuration**: Verify the Dockerfile has the correct setup for Render.
4. **Kestrel Configuration**: Make sure Kestrel is configured to listen on the correct port (from environment).

You can run the included fix script to check for common issues:

```bash
./fixDeployment.sh
```

### 3. Manual Deployment Steps

If you prefer to deploy manually:

1. Clone the repository
2. Build the Docker image:
   ```
   docker build -t todolist-api -f ToDoList_FS/Dockerfile .
   ```
3. Set the required environment variables
4. Deploy to Render using the web service setup

### 4. Troubleshooting

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

The API will be available at `http://localhost:8080`. 