FROM mcr.microsoft.com/dotnet/core/runtime:3.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/PreviewService
RUN dotnet build "PreviewService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PreviewService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PreviewService.dll"]
