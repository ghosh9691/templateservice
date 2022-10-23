FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
EXPOSE 80
EXPOSE 443
WORKDIR /src

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /src
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "TemplateService.dll"]