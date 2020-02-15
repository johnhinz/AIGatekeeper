#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#docker build -t aiguardian .

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . /src
WORKDIR /src/AIGaurd.Service
RUN dotnet restore "AIGaurd.Service.csproj"
RUN dotnet build "AIGaurd.Service.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "AIGaurd.Service.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIGaurd.Service.dll"]