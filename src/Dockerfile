#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["/AIGuard.Orchestrator/AIGuard.Orchestrator.csproj", "AIGuard.Orchestrator/"]
COPY ["/AIGuard.IRepository/AIGuard.IRepository.csproj", "src/AIGuard.IRepository/"]
COPY ["/AIGuard.Broker/AIGuard.Broker.csproj", "src/AIGuard.Broker/"]
COPY ["/AIGuard.Interface/AIGuard.Interface.csproj", "src/AIGuard.Interface/"]
COPY ["/AIGuard.MqttRepository/AIGuard.MqttRepository.csproj", "src/AIGuard.MqttRepository/"]
COPY ["/AIGuard.DeepStack/AIGuard.DeepStack.csproj", "src/AIGuard.DeepStack/"]
       
RUN dotnet restore "AIGuard.Orchestrator/AIGuard.Orchestrator.csproj"

COPY . .

WORKDIR "/src/AIGuard.Orchestrator"
RUN dotnet build "AIGuard.Orchestrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIGuard.Orchestrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIGuard.Orchestrator.dll"]
