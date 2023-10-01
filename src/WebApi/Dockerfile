FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
ARG PRODUCT_VERSION
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["nuget.config", "./"]
COPY ["WebApi/WebApi.csproj", "WebApi/"]
RUN dotnet restore "WebApi/WebApi.csproj"

COPY ["WebApi/", "WebApi/"]
RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app/publish -p:UseAppHost=false -p:Version=${PRODUCT_VERSION} --no-restore --no-self-contained -p:PublishReadyToRun=false

FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-jammy-chiseled-composite
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
