FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
# Angiver det program, der skal køres, når en container startes
# Skal passe til den fil, der ligger i bin-mappen
ENTRYPOINT ["dotnet", "/app/CustomerService.dll"]
