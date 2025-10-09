# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["FinacialBackend.csproj", "."]
RUN dotnet restore "FinacialBackend.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "FinacialBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinacialBackend.dll"]
