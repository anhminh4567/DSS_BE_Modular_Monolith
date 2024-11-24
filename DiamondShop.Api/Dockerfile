#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat
#-nanoserver-1809
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base 
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
#-nanoserver-1809
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build 
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DiamondShop.Api/DiamondShop.Api.csproj", "DiamondShop.Api/"]
COPY ["DiamondShop.Application/DiamondShop.Application.csproj", "DiamondShop.Application/"]
COPY ["DiamondShop.Commons/DiamondShop.Commons.csproj", "DiamondShop.Commons/"]
COPY ["DiamondShop.Domain/DiamondShop.Domain.csproj", "DiamondShop.Domain/"]
COPY ["DiamondShop.Infrastructure/DiamondShop.Infrastructure.csproj", "DiamondShop.Infrastructure/"]
RUN dotnet restore "./DiamondShop.Api/DiamondShop.Api.csproj"
COPY . .
WORKDIR "/src/DiamondShop.Api"
RUN dotnet build "./DiamondShop.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DiamondShop.Api.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiamondShop.Api.dll"]