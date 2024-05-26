FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Client/Client.csproj", "Client/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "./src/Client/Client.csproj"
COPY . .
WORKDIR "/src/src/Client"

RUN dotnet publish "Client.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Client.dll"]