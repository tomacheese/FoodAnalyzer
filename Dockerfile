FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["FoodAnalyzer/FoodAnalyzer.csproj", "FoodAnalyzer/"]
RUN dotnet restore "FoodAnalyzer/FoodAnalyzer.csproj"

COPY . .
WORKDIR "/src/FoodAnalyzer"
RUN dotnet build "FoodAnalyzer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FoodAnalyzer.csproj" -c Release -o /app/publish --self-contained false /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV FOODANALYZER_CONFIG_PATH=/data/config.json
ENV FOODANALYZER_MONITOR_PATH=/data/monitor.json

ENTRYPOINT ["./FoodAnalyzer"]
