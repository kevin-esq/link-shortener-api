# ======================================
# 🏗️ STAGE 1: Build
# ======================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["LinkShortener.Api/LinkShortener.Api.csproj", "LinkShortener.Api/"]
COPY ["LinkShortener.Application/LinkShortener.Application.csproj", "LinkShortener.Application/"]
COPY ["LinkShortener.Domain/LinkShortener.Domain.csproj", "LinkShortener.Domain/"]
COPY ["LinkShortener.Infrastructure/LinkShortener.Infrastructure.csproj", "LinkShortener.Infrastructure/"]

RUN dotnet restore "LinkShortener.Api/LinkShortener.Api.csproj"

# Copy all source code
COPY . .

# Build the project
WORKDIR "/src/LinkShortener.Api"
RUN dotnet build "LinkShortener.Api.csproj" -c Release -o /app/build

# ======================================
# 📦 STAGE 2: Publish
# ======================================
FROM build AS publish
RUN dotnet publish "LinkShortener.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ======================================
# 🚀 STAGE 3: Runtime
# ======================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install OpenSSL to generate RSA keys
RUN apt-get update && apt-get install -y openssl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Create directory for JWT keys
RUN mkdir -p /app/Keys

# Entry script to generate RSA keys if they don't exist
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Expose port
EXPOSE 8080

# Non-root user for security
RUN useradd -m -u 1001 appuser && \
    chown -R appuser:appuser /app && \
    chown -R appuser:appuser /app/Keys

USER appuser

# Entry point
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["dotnet", "LinkShortener.Api.dll"]
