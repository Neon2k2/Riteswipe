# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["RiteSwipe.Api/RiteSwipe.Api.csproj", "RiteSwipe.Api/"]
COPY ["RiteSwipe.Application/RiteSwipe.Application.csproj", "RiteSwipe.Application/"]
COPY ["RiteSwipe.Domain/RiteSwipe.Domain.csproj", "RiteSwipe.Domain/"]
COPY ["RiteSwipe.Infrastructure/RiteSwipe.Infrastructure.csproj", "RiteSwipe.Infrastructure/"]
RUN dotnet restore "RiteSwipe.Api/RiteSwipe.Api.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "RiteSwipe.Api/RiteSwipe.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "RiteSwipe.Api/RiteSwipe.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "RiteSwipe.Api.dll"]
