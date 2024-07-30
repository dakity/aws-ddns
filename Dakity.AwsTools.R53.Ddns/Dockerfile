#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dakity.AwsTools.R53.Ddns/Dakity.AwsTools.R53.Ddns.csproj", "Dakity.AwsTools.R53.Ddns/"]
RUN dotnet restore "./Dakity.AwsTools.R53.Ddns/Dakity.AwsTools.R53.Ddns.csproj"
COPY . .
WORKDIR "/src/Dakity.AwsTools.R53.Ddns"
RUN dotnet build "./Dakity.AwsTools.R53.Ddns.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dakity.AwsTools.R53.Ddns.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dakity.AwsTools.R53.Ddns.dll"]