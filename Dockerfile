# Copyright 2023 mixayloff-dimaaylov at github dot com
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

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
