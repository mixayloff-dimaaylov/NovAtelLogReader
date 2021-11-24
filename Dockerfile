# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY NovAtelLogReader/*.sln .
COPY NovAtelLogReader/NovAtelLogReader/NovAtelLogReader.csproj ./NovAtelLogReader/
COPY NovAtelLogReader/NovAtelRunner/NovAtelRunner.csproj ./NovAtelRunner/
RUN dotnet restore

# copy everything else and build app
COPY NovAtelLogReader/. ./NovAtelLogReader/
WORKDIR /source/NovAtelLogReader
RUN dotnet build NovAtelLogReader --configuration Release
RUN dotnet build NovAtelRunner --configuration Release

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /source/NovAtelLogReader/Release/netcoreapp6.0/. .
ENTRYPOINT "/app/NovAtelLogReader"
