FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UberPopug.AuthService/UberPopug.AuthService.csproj", "UberPopug.AuthService/"]
RUN dotnet restore "UberPopug.AuthService/UberPopug.AuthService.csproj"
COPY . .
WORKDIR "/src/UberPopug.AuthService"
RUN dotnet build "UberPopug.AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UberPopug.AuthService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UberPopug.AuthService.dll"]
