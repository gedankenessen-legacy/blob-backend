#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Blob_API/Blob_API.csproj", "Blob_API/"]
RUN dotnet restore "Blob_API/Blob_API.csproj"
COPY . .
WORKDIR "/src/Blob_API"
RUN dotnet build "Blob_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blob_API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blob_API.dll"]