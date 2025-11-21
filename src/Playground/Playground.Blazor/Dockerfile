ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Copy project files first to maximize restore layer caching
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Playground/Playground.Blazor/Playground.Blazor.csproj", "src/Playground/Playground.Blazor/"]
COPY ["src/BuildingBlocks/Blazor.UI/Blazor.UI.csproj", "src/BuildingBlocks/Blazor.UI/"]

RUN dotnet restore "src/Playground/Playground.Blazor/Playground.Blazor.csproj"

# Now copy the full source and publish
COPY . .
WORKDIR "/src/src/Playground/Playground.Blazor"
RUN dotnet publish "Playground.Blazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FSH.Playground.Blazor.dll"]

