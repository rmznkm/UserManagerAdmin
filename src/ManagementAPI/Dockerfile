#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ManagementAPI/ManagementAPI.csproj", "src/ManagementAPI/"]
COPY ["src/EventBus/EventBus.csproj", "src/EventBus/"]
RUN dotnet restore "src/ManagementAPI/ManagementAPI.csproj"
COPY . .
WORKDIR "/src/src/ManagementAPI"
RUN dotnet build "ManagementAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ManagementAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManagementAPI.dll"]
