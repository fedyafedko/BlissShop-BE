FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:80
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlissShop/BlissShop.csproj", "BlissShop/"]
COPY ["BlissShop.BLL/BlissShop.BLL.csproj", "BlissShop.BLL/"]
COPY ["BlissShop.Abstraction/BlissShop.Abstraction.csproj", "BlissShop.Abstraction/"]
COPY ["BlissShop.Common/BlissShop.Common.csproj", "BlissShop.Common/"]
COPY ["BlissShop.Entities/BlissShop.Entities.csproj", "BlissShop.Entities/"]
COPY ["BlissShop.DAL/BlissShop.DAL.csproj", "BlissShop.DAL/"]
COPY ["BlissShop.FluentEmail/BlissShop.FluentEmail.csproj", "BlissShop.FluentEmail/"]
COPY ["BlissShop.Hangfire/BlissShop.Hangfire.csproj", "BlissShop.Hangfire/"]
COPY ["BlissShop.Seeding/BlissShop.Seeding.csproj", "BlissShop.Seeding/"]
COPY ["BlissShop.Validation/BlissShop.Validation.csproj", "BlissShop.Validation/"]
RUN dotnet restore "./BlissShop/BlissShop.csproj"
COPY . .
WORKDIR "/src/BlissShop"
RUN dotnet build "./BlissShop.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlissShop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlissShop.dll"]