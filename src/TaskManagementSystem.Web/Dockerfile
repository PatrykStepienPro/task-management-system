FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TaskManagementSystem.Web/TaskManagementSystem.Web.csproj", "src/TaskManagementSystem.Web/"]
RUN dotnet restore "src/TaskManagementSystem.Web/TaskManagementSystem.Web.csproj"
COPY . .
WORKDIR "/src/src/TaskManagementSystem.Web"
RUN dotnet build "TaskManagementSystem.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagementSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagementSystem.Web.dll"]
