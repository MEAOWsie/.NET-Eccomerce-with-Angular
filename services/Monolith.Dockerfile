FROM node:20-alpine AS client
WORKDIR /src/client
COPY client/package*.json ./
RUN npm install
COPY client/ .
RUN npx ng build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
COPY --from=client /src/API/wwwroot ./API/wwwroot
RUN dotnet publish API/API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]
