FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

COPY ["Client/Client.csproj", "Client/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "/Client/Client.csproj"
COPY . .
WORKDIR "/Client"

RUN dotnet publish "Client.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

# ENTRYPOINT ["dotnet", "smart-ecard-frontend.Client.dll"]
ENTRYPOINT ["dotnet", "Client.dll"]
