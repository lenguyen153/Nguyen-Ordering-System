# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "OrderManagementSystem.csproj"
RUN dotnet publish "OrderManagementSystem.csproj" -c Release -o /app/publish

# Use the official .NET 8 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Expose port 5057 for HTTP
EXPOSE 5057
ENV ASPNETCORE_URLS=http://+:5057
ENTRYPOINT ["dotnet", "OrderManagementSystem.dll"]
