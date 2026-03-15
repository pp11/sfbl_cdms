#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["SalesAndDistributionSystem/SalesAndDistributionSystem.csproj", "SalesAndDistributionSystem/"]
COPY ["SalesAndDistributionSystem.Domain/SalesAndDistributionSystem.Domain.csproj", "SalesAndDistributionSystem.Domain/"]
COPY ["SalesAndDistributionSystem.Services/SalesAndDistributionSystem.Services.csproj", "SalesAndDistributionSystem.Services/"]
RUN dotnet restore "SalesAndDistributionSystem/SalesAndDistributionSystem.csproj"
COPY . .
WORKDIR "/src/SalesAndDistributionSystem"
RUN dotnet build "SalesAndDistributionSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesAndDistributionSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesAndDistributionSystem.dll"]