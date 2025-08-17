# Rentify.RazorWebApp/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Ứng dụng Kestrel nghe trên 8080 trong container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --retries=5 CMD wget -qO- http://localhost:8080/ || exit 1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Rentify.RazorWebApp/Rentify.RazorWebApp.csproj", "Rentify.RazorWebApp/"]
COPY ["Rentify.Services/Rentify.Services.csproj", "Rentify.Services/"]
COPY ["Rentify.Repositories/Rentify.Repositories.csproj", "Rentify.Repositories/"]
COPY ["Rentify.BusinessObjects/Rentify.BusinessObjects.csproj", "Rentify.BusinessObjects/"]
RUN dotnet restore "Rentify.RazorWebApp/Rentify.RazorWebApp.csproj"

COPY . .
WORKDIR "/src/Rentify.RazorWebApp"
RUN dotnet build "./Rentify.RazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Rentify.RazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rentify.RazorWebApp.dll"]
