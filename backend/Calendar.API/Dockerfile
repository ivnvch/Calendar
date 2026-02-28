FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Calendar.API/Calendar.API.csproj", "Calendar.API/"]
COPY ["Calendar.Infrastructure.Postgres/Calendar.Infrastructure.Postgres.csproj", "Calendar.Infrastructure.Postgres/"]
COPY ["Calendar.Domain/Calendar.Domain.csproj", "Calendar.Domain/"]
COPY ["Calendar.Shared/Calendar.Shared.csproj", "Calendar.Shared/"]
COPY ["Calendar.Application/Calendar.Application.csproj", "Calendar.Application/"]
RUN dotnet restore "Calendar.API/Calendar.API.csproj"
COPY . .
WORKDIR "/src/Calendar.API"
RUN dotnet build "./Calendar.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Calendar.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calendar.API.dll"]
