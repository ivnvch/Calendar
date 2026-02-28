FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY *.sln .
COPY Calendar.API/*.csproj Calendar.API/
COPY Calendar.Application/*.csproj Calendar.Application/
COPY Calendar.Domain/*.csproj Calendar.Domain/
COPY Calendar.Infrastructure.Postgres/*.csproj Calendar.Infrastructure.Postgres/
COPY Calendar.Shared/*.csproj Calendar.Shared/
RUN dotnet restore

COPY . .
RUN dotnet publish Calendar.API/Calendar.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Calendar.API.dll"]
