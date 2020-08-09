##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
#WORKDIR /app
#
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#WORKDIR /src
#COPY ["AIGuard.MySQLSubscriber/AIGuard.MySQLSubscriber.csproj", "AIGuard.MySQLSubscriber/"]
#COPY ["AIGuard.MySQLRepository/AIGuard.MySQLRepository.csproj", "AIGuard.MySQLRepository/"]
#COPY ["AIGuard.IRepository/AIGuard.IRepository.csproj", "AIGuard.IRepository/"]
#COPY ["AIGuard.Broker/AIGuard.Broker.csproj", "AIGuard.Broker/"]
#RUN dotnet restore "AIGuard.MySQLSubscriber/AIGuard.MySQLSubscriber.csproj"
#COPY . .
#WORKDIR "/src/AIGuard.MySQLSubscriber"
#RUN dotnet build "AIGuard.MySQLSubscriber.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "AIGuard.MySQLSubscriber.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "AIGuard.MySQLSubscriber.dll"]
#