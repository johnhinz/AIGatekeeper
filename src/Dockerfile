#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/AIGuard.Orchestrator/AIGuard.Orchestrator.csproj", "AIGuard.Orchestrator/"]
COPY ["src/AIGuard.IRepository/AIGuard.IRepository.csproj", "src/AIGuard.IRepository/"]
COPY ["src/AIGuard.Broker/AIGuard.Broker.csproj", "src/AIGuard.Broker/"]
COPY ["src/AIGuard.Interface/AIGuard.Interface.csproj", "src/AIGuard.Interface/"]
COPY ["src/AIGuard.MqttRepository/AIGuard.MqttRepository.csproj", "src/AIGuard.MqttRepository/"]
COPY ["src/AIGuard.DeepStack/AIGuard.DeepStack.csproj", "src/AIGuard.DeepStack/"]
       
RUN dotnet restore "src/AIGuard.Orchestrator/AIGuard.Orchestrator.csproj"

COPY . .

WORKDIR "/src/AIGuard.Orchestrator"
RUN dotnet build "AIGuard.Orchestrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIGuard.Orchestrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIGuard.Orchestrator.dll"]