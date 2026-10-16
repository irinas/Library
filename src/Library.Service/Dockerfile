FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/Library.Service/Library.Service.csproj
RUN dotnet publish src/Library.Service/Library.Service.csproj -c Release -o /app/publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /data && chown -R $APP_UID:$APP_UID /data
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080 ASPNETCORE_HTTP_PORTS=8080 ConnectionStrings__Library="Data Source=/data/library.db;Cache=Shared"
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet","Library.Service.dll"]
