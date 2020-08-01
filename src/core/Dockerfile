FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/core/core.csproj", "src/core/"]
RUN dotnet restore "src/core/core.csproj"
COPY . .
WORKDIR "/src/src/core"
RUN dotnet build "core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "core.dll"]