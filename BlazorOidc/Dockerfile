#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_HTTP_PORTS=80

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlazorOidc/BlazorOidc.csproj", "BlazorOidc/"]
RUN dotnet restore "./BlazorOidc/BlazorOidc.csproj"
COPY . .
WORKDIR "/src/BlazorOidc"
RUN dotnet build "./BlazorOidc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorOidc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN apk upgrade musl

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorOidc.dll"]
