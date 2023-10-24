FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Ingredients/Ingredients.csproj", "Ingredients/"]
RUN dotnet restore "Ingredients/Ingredients.csproj"
COPY . .
WORKDIR "/src/Ingredients"
RUN dotnet build "Ingredients.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ingredients.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ingredients.dll"]
