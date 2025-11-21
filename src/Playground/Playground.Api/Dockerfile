ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Copy project files first to maximize restore layer caching
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Playground/Playground.Api/Playground.Api.csproj", "src/Playground/Playground.Api/"]
COPY ["src/Playground/Migrations.PostgreSQL/Migrations.PostgreSQL.csproj", "src/Playground/Migrations.PostgreSQL/"]
COPY ["src/BuildingBlocks/Web/Web.csproj", "src/BuildingBlocks/Web/"]
COPY ["src/BuildingBlocks/Caching/Caching.csproj", "src/BuildingBlocks/Caching/"]
COPY ["src/BuildingBlocks/Core/Core.csproj", "src/BuildingBlocks/Core/"]
COPY ["src/BuildingBlocks/Jobs/Jobs.csproj", "src/BuildingBlocks/Jobs/"]
COPY ["src/BuildingBlocks/Mailing/Mailing.csproj", "src/BuildingBlocks/Mailing/"]
COPY ["src/BuildingBlocks/Persistence/Persistence.csproj", "src/BuildingBlocks/Persistence/"]
COPY ["src/BuildingBlocks/Shared/Shared.csproj", "src/BuildingBlocks/Shared/"]
COPY ["src/BuildingBlocks/Storage/Storage.csproj", "src/BuildingBlocks/Storage/"]
COPY ["src/Modules/Auditing/Modules.Auditing/Modules.Auditing.csproj", "src/Modules/Auditing/Modules.Auditing/"]
COPY ["src/Modules/Auditing/Modules.Auditing.Contracts/Modules.Auditing.Contracts.csproj", "src/Modules/Auditing/Modules.Auditing.Contracts/"]
COPY ["src/Modules/Identity/Modules.Identity/Modules.Identity.csproj", "src/Modules/Identity/Modules.Identity/"]
COPY ["src/Modules/Identity/Modules.Identity.Contracts/Modules.Identity.Contracts.csproj", "src/Modules/Identity/Modules.Identity.Contracts/"]
COPY ["src/Modules/Multitenancy/Modules.Multitenancy/Modules.Multitenancy.csproj", "src/Modules/Multitenancy/Modules.Multitenancy/"]
COPY ["src/Modules/Multitenancy/Modules.Multitenancy.Contracts/Modules.Multitenancy.Contracts.csproj", "src/Modules/Multitenancy/Modules.Multitenancy.Contracts/"]

RUN dotnet restore "src/Playground/Playground.Api/Playground.Api.csproj"

# Now copy the full source and publish
COPY . .
WORKDIR "/src/src/Playground/Playground.Api"
RUN dotnet publish "Playground.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FSH.Playground.Api.dll"]

