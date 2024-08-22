FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80  # Expondo a porta HTTP padrão
EXPOSE 8080 # Expondo a porta 8080 se for usada para HTTP

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VShop.Web/VShop.Web.csproj", "VShop.Web/"]
RUN dotnet restore "./VShop.Web/VShop.Web.csproj"
COPY . .
WORKDIR "/src/VShop.Web"
RUN dotnet build "./VShop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VShop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VShop.Web.dll"]
