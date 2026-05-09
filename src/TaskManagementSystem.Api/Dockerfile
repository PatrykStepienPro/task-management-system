FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TaskManagementSystem.Api/TaskManagementSystem.Api.csproj", "src/TaskManagementSystem.Api/"]
COPY ["src/TaskManagementSystem.Application/TaskManagementSystem.Application.csproj", "src/TaskManagementSystem.Application/"]
COPY ["src/TaskManagementSystem.Domain/TaskManagementSystem.Domain.csproj", "src/TaskManagementSystem.Domain/"]
COPY ["src/TaskManagementSystem.Infrastructure/TaskManagementSystem.Infrastructure.csproj", "src/TaskManagementSystem.Infrastructure/"]
RUN dotnet restore "src/TaskManagementSystem.Api/TaskManagementSystem.Api.csproj"
COPY . .
WORKDIR "/src/src/TaskManagementSystem.Api"
RUN dotnet build "TaskManagementSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagementSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagementSystem.Api.dll"]
