# File will change its just a placeholder so we get it to run
# Stage 1: Build the app using .NET SDK 6.0
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy the project files to the working directory of the docker image
COPY . ./
RUN dotnet publish Ingredients/Ingredients.csproj -c Release -o out

# Stage 2: Use the official .NET 6.0 runtime base image for better performance
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy the build output from the previous stage
COPY --from=build-env /app/out .

# Specify the entry point command to start the app
ENTRYPOINT ["dotnet", "Ingredients.dll"]