FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SmartOps.Domain/SmartOps.Domain.csproj", "SmartOps.Domain/"]
COPY ["SmartOps.Application/SmartOps.Application.csproj", "SmartOps.Application/"]
COPY ["SmartOps.Infrastructure/SmartOps.Infrastructure.csproj", "SmartOps.Infrastructure/"]
COPY ["SmartOps.API/SmartOps.API.csproj", "SmartOps.API/"]

RUN dotnet restore "SmartOps.API/SmartOps.API.csproj"

COPY . .

WORKDIR /src/SmartOps.API

RUN dotnet publish "SmartOps.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartOps.API.dll"]
