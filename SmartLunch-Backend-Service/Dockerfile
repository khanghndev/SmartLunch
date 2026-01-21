FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

COPY SmartLunch-Backend-Service/SmartLunch-Backend-Service-API/SmartLunch.Backend.Service.API.csproj SmartLunch-Backend-Service/SmartLunch-Backend-Service-API/
COPY SmartLunch-Backend-Service/SmartLunch-Backend-Service-Application/*.csproj SmartLunch-Backend-Service/SmartLunch-Backend-Service-Application/
COPY SmartLunch-Backend-Service/SmartLunch-Backend-Service-Domain/*.csproj SmartLunch-Backend-Service/SmartLunch-Backend-Service-Domain/
COPY SmartLunch-Backend-Service/SmartLunch-Backend-Service-Infrastructure/*.csproj SmartLunch-Backend-Service/SmartLunch-Backend-Service-Infrastructure/
COPY SmartLunch-Shared-MessageQueue-Dotnet/*.csproj SmartLunch-Shared-MessageQueue-Dotnet/

RUN dotnet restore SmartLunch-Backend-Service/SmartLunch-Backend-Service-API/SmartLunch.Backend.Service.API.csproj

COPY SmartLunch-Backend-Service/ SmartLunch-Backend-Service/
COPY SmartLunch-Shared-MessageQueue-Dotnet/ SmartLunch-Shared-MessageQueue-Dotnet/

RUN dotnet publish SmartLunch-Backend-Service/SmartLunch-Backend-Service-API/SmartLunch.Backend.Service.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SmartLunch.Backend.Service.API.dll"]