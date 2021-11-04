FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UberPopug.TaskTrackerService/UberPopug.TaskTrackerService.csproj", "UberPopug.TaskTrackerService/"]
RUN dotnet restore "UberPopug.TaskTrackerService/UberPopug.TaskTrackerService.csproj"
COPY . .
WORKDIR "/src/UberPopug.TaskTrackerService"
RUN dotnet build "UberPopug.TaskTrackerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UberPopug.TaskTrackerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UberPopug.TaskTrackerService.dll"]
