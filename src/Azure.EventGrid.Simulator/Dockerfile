#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Azure.EventGrid.Simulator/Azure.EventGrid.Simulator.csproj", "src/Azure.EventGrid.Simulator/"]
RUN dotnet restore "src/Azure.EventGrid.Simulator/Azure.EventGrid.Simulator.csproj"
COPY . .
WORKDIR "/src/src/Azure.EventGrid.Simulator"
RUN dotnet build "Azure.EventGrid.Simulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Azure.EventGrid.Simulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Azure.EventGrid.Simulator.dll"]