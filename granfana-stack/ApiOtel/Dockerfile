FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ApiOtel.csproj .
RUN dotnet restore ApiOtel.csproj

COPY . .
RUN dotnet build ApiOtel.csproj --no-restore --configuration Release --output ./build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build-env /app/build .
CMD ["dotnet", "ApiOtel.dll"]