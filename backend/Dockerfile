FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Paper.Domain/Paper.Domain.csproj Paper.Domain/
COPY Paper.Application/Paper.Application.csproj Paper.Application/
COPY Paper.Infrastructure/Paper.Infrastructure.csproj Paper.Infrastructure/
COPY PaperAPI/PaperAPI.csproj PaperAPI/
RUN dotnet restore PaperAPI/PaperAPI.csproj

COPY Paper.Domain/ Paper.Domain/
COPY Paper.Application/ Paper.Application/
COPY Paper.Infrastructure/ Paper.Infrastructure/
COPY PaperAPI/ PaperAPI/
RUN dotnet publish PaperAPI/PaperAPI.csproj -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaperAPI.dll"]
