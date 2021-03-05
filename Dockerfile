FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layer
COPY *.sln .
COPY src/TemplateService/*.csproj ./src/TemplateService/
COPY tests/TemplateService.Tests/*.csproj ./tests/TemplateService.Tests/
RUN dotnet restore

# copy everything else to build the app
COPY src/TemplateService/. ./src/TemplateService/
COPY tests/TemplateService.Tests/. ./tests/TemplateService.Tests/
WORKDIR /source/src/TemplateService
RUN dotnet build

# create a layer for unit tests
FROM build AS test
WORKDIR /source/tests/TemplateService.Tests
RUN dotnet test

# now create publish layer
FROM build AS publish
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=publish /app ./
EXPOSE 80
ENV ASPNETCORE_URLS="http://+:80"
ENTRYPOINT ["dotnet", "TemplateService.dll"]

