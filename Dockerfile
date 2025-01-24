# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY MangoWallet.sln ./
COPY BL/BL.csproj ./BL/
COPY DAL/DAL.csproj ./DAL/
COPY Domain/Domain.csproj ./Domain/
COPY UI.MVC/UI.MVC.csproj ./UI.MVC/
RUN dotnet restore

COPY . ./
RUN dotnet publish UI.MVC/UI.MVC.csproj -c Release -o out

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# **Create the /app/db directory** to store your database file
RUN mkdir -p /app/db
# If needed, give write permissions:
RUN chmod 777 /app/db

COPY --from=build /app/out ./
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UI.MVC.dll"]