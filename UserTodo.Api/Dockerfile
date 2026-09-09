FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserTodo.Api/UserTodo.Api.csproj", "UserTodo.Api/"]
RUN dotnet restore "UserTodo.Api/UserTodo.Api.csproj"
COPY . .
WORKDIR "/src/UserTodo.Api"
RUN dotnet build "UserTodo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserTodo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserTodo.Api.dll"]
