#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/OrderService/SellingBuddy.OrderService.Api/SellingBuddy.OrderService.Api.csproj", "src/Services/OrderService/SellingBuddy.OrderService.Api/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Base/SellingBuddy.EventBus.Base.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Base/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Factory/SellingBuddy.EventBus.Factory.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.Factory/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.AzureServiceBus/SellingBuddy.EventBus.AzureServiceBus.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.AzureServiceBus/"]
COPY ["src/BuildingBlocks/EventBus/SellingBuddy.EventBus.RabbitMQ/SellingBuddy.EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus/SellingBuddy.EventBus.RabbitMQ/"]
COPY ["src/Services/OrderService/SellingBuddy.OrderService.Application/SellingBuddy.OrderService.Application.csproj", "src/Services/OrderService/SellingBuddy.OrderService.Application/"]
COPY ["src/Services/OrderService/SellingBuddy.OrderService.Domain/SellingBuddy.OrderService.Domain.csproj", "src/Services/OrderService/SellingBuddy.OrderService.Domain/"]
COPY ["src/Services/OrderService/SellingBuddy.OrderService.Infrastructure/SellingBuddy.OrderService.Infrastructure.csproj", "src/Services/OrderService/SellingBuddy.OrderService.Infrastructure/"]
RUN dotnet restore "./src/Services/OrderService/SellingBuddy.OrderService.Api/./SellingBuddy.OrderService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/OrderService/SellingBuddy.OrderService.Api"
RUN dotnet build "./SellingBuddy.OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SellingBuddy.OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SellingBuddy.OrderService.Api.dll"]