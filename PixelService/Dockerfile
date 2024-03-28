FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PixelService/PixelService.csproj", "PixelService/"]
RUN dotnet restore "./PixelService/PixelService.csproj"
COPY . .
WORKDIR "/src/PixelService"
RUN dotnet build "./PixelService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./PixelService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelService.dll"]