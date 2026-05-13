# Build context: repo root
# Usage: docker build -f backend/RcCloud.WebApi/Dockerfile -t rc-cloud-webapi .

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/RcCloud.DateScraper.Domain/RcCloud.DateScraper.Domain.csproj backend/RcCloud.DateScraper.Domain/
COPY backend/RcCloud.DateScraper.Infrastructure/RcCloud.DateScraper.Infrastructure.csproj backend/RcCloud.DateScraper.Infrastructure/
COPY backend/RcCloud.DateScraper.Application.Common/RcCloud.DateScraper.Application.Common.csproj backend/RcCloud.DateScraper.Application.Common/
COPY backend/RcCloud.DateScraper.Application/RcCloud.DateScraper.Application.csproj backend/RcCloud.DateScraper.Application/
COPY backend/RcCloud.DateScraper.Application.Dmc/RcCloud.DateScraper.Application.Dmc.csproj backend/RcCloud.DateScraper.Application.Dmc/
COPY backend/RcCloud.DateScraper.Application.Myrcm/RcCloud.DateScraper.Application.Myrcm.csproj backend/RcCloud.DateScraper.Application.Myrcm/
COPY backend/RcCloud.DateScraper.Application.Rcco/RcCloud.DateScraper.Application.Rcco.csproj backend/RcCloud.DateScraper.Application.Rcco/
COPY backend/RcCloud.DateScraper.Application.Rck/RcCloud.DateScraper.Application.Rck.csproj backend/RcCloud.DateScraper.Application.Rck/
COPY backend/RcCloud.WebApi/RcCloud.WebApi.csproj backend/RcCloud.WebApi/
COPY provider-dmc/RcCloud.Provider.Dmc/RcCloud.Provider.Dmc.csproj provider-dmc/RcCloud.Provider.Dmc/

RUN dotnet restore backend/RcCloud.WebApi/RcCloud.WebApi.csproj

COPY backend/ backend/
COPY provider-dmc/ provider-dmc/

RUN dotnet publish backend/RcCloud.WebApi/RcCloud.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RcCloud.WebApi.dll"]
