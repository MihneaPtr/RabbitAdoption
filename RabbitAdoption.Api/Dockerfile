FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/RabbitAdoption.Api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Set ASPNETCORE_URLS to listen on all interfaces and port 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "RabbitAdoption.Api.dll"]