# Sử dụng .NET 10 SDK làm build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Cài đặt Node.js và npm (bắt buộc vì .csproj chạy npm ci và build css:build để sinh file tailwind.css)
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Copy file project và restore các NuGet packages
COPY ["e-commerce-web-customer.csproj", "./"]
RUN dotnet restore "e-commerce-web-customer.csproj"

# Copy toàn bộ mã nguồn
COPY . .

# Build ứng dụng (lệnh này sẽ tự chạy 'npm ci' và 'npm run css:build' thông qua các target cấu hình trong csproj)
RUN dotnet build "e-commerce-web-customer.csproj" -c Release -o /app/build

# Publish ứng dụng ra thư mục đầu ra
FROM build AS publish
RUN dotnet publish "e-commerce-web-customer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage final: Chạy ứng dụng bằng ASP.NET Core Runtime 10.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cấu hình cổng chạy mặc định cho ASP.NET Core (.NET 8/10 sử dụng mặc định 8080 trong Docker)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "e-commerce-web-customer.dll"]
