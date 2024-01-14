#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/NotificationService/SellingBuddy.NotificationService.Console/SellingBuddy.NotificationService.Console.csproj", "src/Services/NotificationService/SellingBuddy.NotificationService.Console/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Base/SellingBuddy.EventBus.Base.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Base/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Factory/SellingBuddy.EventBus.Factory.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Factory/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.AzureServiceBus/SellingBuddy.EventBus.AzureServiceBus.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.AzureServiceBus/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.RabbitMQ/SellingBuddy.EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.RabbitMQ/"]
RUN dotnet restore "./src/Services/NotificationService/SellingBuddy.NotificationService.Console/./SellingBuddy.NotificationService.Console.csproj"
COPY . .
WORKDIR "/src/src/Services/NotificationService/SellingBuddy.NotificationService.Console"
RUN dotnet build "./SellingBuddy.NotificationService.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SellingBuddy.NotificationService.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SellingBuddy.NotificationService.Console.dll"]