FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["CareerPath/CareerPath.csproj", "CareerPath/"]
COPY ["CareerPath.DataAccess/CareerPath.DataAccess.csproj", "CareerPath.DataAccess/"]
COPY ["CareerPath.Models/CareerPath.Models.csproj", "CareerPath.Models/"]
COPY ["CareerPath.Services/CareerPath.Services.csproj", "CareerPath.Services/"]
COPY ["CareerPath.Utilities/CareerPath.Utilities.csproj", "CareerPath.Utilities/"]

RUN dotnet restore "CareerPath/CareerPath.csproj"

COPY . .

RUN dotnet publish "CareerPath/CareerPath.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CareerPath.dll"]
