
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["N5Challenge.API/N5Challenge.API.csproj", "N5Challenge.API/"]
COPY ["N5Challenge.Application/N5Challenge.Application.csproj", "N5Challenge.Application/"]
COPY ["N5Challenge.Core/N5Challenge.Core.csproj", "N5Challenge.Core/"]
COPY ["N5Challenge.Infrastructure/N5Challenge.Infrastructure.csproj", "N5Challenge.Infrastructure/"]
RUN dotnet restore "N5Challenge.API/N5Challenge.API.csproj"
COPY . .
WORKDIR "/src/N5Challenge.API"
RUN dotnet build "N5Challenge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "N5Challenge.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "N5Challenge.API.dll"]