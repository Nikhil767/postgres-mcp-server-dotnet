# ===================================
# Stage 1: Build Stage
# Uses full SDK image to restore, build, and publish
# ===================================
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy project files first for better cache usage
COPY ["PostgresMcpServer.slnx", "./"]
COPY ["src/PostgresMcpServer/PostgresMcpServer.csproj", "src/PostgresMcpServer/"]
COPY ["tests/PostgresMcpServer.Tests/PostgresMcpServer.Tests.csproj", "tests/PostgresMcpServer.Tests/"]

# Restore both projects
RUN dotnet restore "PostgresMcpServer.slnx"

# Copy remaining source code
COPY . .

# Run unit tests
# If tests fail, Docker build will stop here
RUN dotnet build "tests/PostgresMcpServer.Tests/PostgresMcpServer.Tests.csproj" -c Release --no-restore
RUN dotnet test "tests/PostgresMcpServer.Tests/PostgresMcpServer.Tests.csproj" -c Release --no-build

# ===================================
# Stage 2: Publish Stage
# Creates self-contained, trimmed deployment
# ===================================
FROM build AS publish
RUN dotnet publish "src/PostgresMcpServer/PostgresMcpServer.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ===================================
# Stage 3: Runtime Stage (Final Image)
# Uses the secure, .NET 10 Alpine ASP.NET runtime
# ===================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Run as non-root user for security
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D appuser
USER appuser

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose port (informational only)
EXPOSE 8080

# Set ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PostgresMcpServer.dll"]
