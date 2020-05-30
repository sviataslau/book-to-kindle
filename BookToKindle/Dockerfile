FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base

# Install Calibre
ARG APT_OPTIONS="-o Acquire::Check-Valid-Until=false -o Acquire::Check-Date=false"
RUN apt-get ${APT_OPTIONS} update && apt-get install -y python && apt-get install -y libgl1-mesa-glx && apt-get install -y libfontconfig
RUN curl -nv https://download.calibre-ebook.com/linux-installer.sh | sh /dev/stdin

# App build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY BookToKindle/ /src/BookToKindle/
WORKDIR /src/BookToKindle
RUN dotnet restore BookToKindle.csproj

# App startup
FROM build AS publish
WORKDIR /src/BookToKindle
RUN dotnet publish "BookToKindle.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookToKindle.dll"]