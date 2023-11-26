FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Ingredients.csproj", "Ingredients.csproj"]
RUN dotnet restore "Ingredients.csproj"
COPY . .
WORKDIR "/src/Ingredients"
RUN dotnet build "Ingredients.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ingredients.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ingredients.dll"]
