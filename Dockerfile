FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /

# Copy csproj files and restore dependencies
COPY ["API/API.csproj", "API/"]
RUN dotnet restore "API/API.csproj"

# Copy the rest of the code
COPY . .

# Build the application
RUN dotnet build "API/API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "API.dll"] 