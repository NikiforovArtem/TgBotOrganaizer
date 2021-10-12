#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TgBotOrganaizer/TgBotOrganaizer.csproj", "TgBotOrganaizer/"]
RUN dotnet restore "TgBotOrganaizer/TgBotOrganaizer.csproj"
COPY . .
WORKDIR "/src/TgBotOrganaizer"
RUN dotnet build "TgBotOrganaizer.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "TgBotOrganaizer.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TgBotOrganaizer.dll"]


