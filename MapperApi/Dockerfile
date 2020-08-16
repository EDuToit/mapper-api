FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish "Mapper Api.csproj" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2
EXPOSE 5001
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Mapper Api.dll"]