FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Mirage.Urbanization.Web/Mirage.Urbanization.Web.csproj", "Mirage.Urbanization.Web/"]
COPY ["Mirage.Urbanization/Mirage.Urbanization.csproj", "Mirage.Urbanization/"]
COPY ["Mirage.Urbanization.Simulation/Mirage.Urbanization.Simulation.csproj", "Mirage.Urbanization.Simulation/"]
COPY ["Mirage.Urbanization.Charts/Mirage.Urbanization.Charts.csproj", "Mirage.Urbanization.Charts/"]
COPY ["Mirage.Urbanization.Resources/Mirage.Urbanization.Tilesets.csproj", "Mirage.Urbanization.Resources/"]
RUN dotnet restore "Mirage.Urbanization.Web/Mirage.Urbanization.Web.csproj"
COPY . .
WORKDIR "/src/Mirage.Urbanization.Web"
RUN dotnet tool install -g Microsoft.Web.LibraryManager.CLI
ENV PATH="$PATH:/root/.dotnet/tools"
RUN libman restore
RUN dotnet build "Mirage.Urbanization.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mirage.Urbanization.Web.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 5000
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mirage.Urbanization.Web.dll"]