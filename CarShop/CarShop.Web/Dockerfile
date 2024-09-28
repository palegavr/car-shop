#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CarShop.Web/CarShop.Web.csproj", "CarShop.Web/"]
COPY ["CarShop.ServiceDefaults/CarShop.ServiceDefaults.csproj", "CarShop.ServiceDefaults/"]
RUN dotnet restore "./CarShop.Web/CarShop.Web.csproj"
COPY . .
WORKDIR "/src/CarShop.Web"
RUN dotnet build "./CarShop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CarShop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarShop.Web.dll"]