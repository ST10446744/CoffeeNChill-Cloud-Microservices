FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base

WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["CoffeeNChill.Functions.csproj", "./"]

RUN dotnet restore "CoffeeNChill.Functions.csproj"

COPY . .

RUN dotnet publish "CoffeeNChill.Functions.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /home/site/wwwroot

COPY --from=build /app/publish .

ENV AzureWebJobsScriptRoot=/home/site/wwwroot
ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true
