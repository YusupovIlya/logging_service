#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Logging.Service.WebApi/Logging.Service.WebApi.csproj", "src/Logging.Service.WebApi/"]
COPY ["src/Logging.Service.Models/Logging.Service.Models.csproj", "src/Logging.Service.Models/"]
COPY ["src/Logging.Service.Validator/Logging.Service.Validator.csproj", "src/Logging.Service.Validator/"]
RUN dotnet restore "./src/Logging.Service.WebApi/./Logging.Service.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Logging.Service.WebApi"
RUN dotnet build "./Logging.Service.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Logging.Service.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logging.Service.WebApi.dll"]