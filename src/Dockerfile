FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish Mirage.Urbanization.Web -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Mirage.Urbanization.Web.dll"]