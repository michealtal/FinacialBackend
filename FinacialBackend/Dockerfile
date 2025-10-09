FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FinacialBackend/FinacialBackend.csproj", "FinacialBackend/"]
RUN dotnet restore "FinacialBackend/FinacialBackend.csproj"

COPY . .
WORKDIR "/src/FinacialBackend"
RUN dotnet publish "FinacialBackend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinacialBackend.dll"]
