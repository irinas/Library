FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/Library.Api/Library.Api.csproj
RUN dotnet publish src/Library.Api/Library.Api.csproj -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080 ASPNETCORE_HTTP_PORTS=8080 LibraryService__Address=http://library-service:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet","Library.Api.dll"]
