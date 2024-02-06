FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.props .
COPY *.sln .

COPY BLL/*.csproj ./BLL/
COPY DAL/*.csproj ./DAL/
COPY Domain/*.csproj ./Domain/
COPY DTO/*.csproj ./DTO/
COPY Helpers/*.csproj ./Helpers/
COPY Tests/*.csproj ./Tests/
COPY WebApp/*.csproj ./WebApp/

RUN dotnet restore


# copy everything else and build app
COPY BLL/. ./BLL/
COPY DAL/. ./DAL/
COPY Domain/. ./Domain/
COPY DTO/. ./DTO/
COPY Helpers/. ./Helpers/
COPY Tests/. ./Tests/
COPY WebApp/. ./WebApp/

WORKDIR /src/WebApp
RUN dotnet publish -c Release -o out



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /src/WebApp/out ./

ENTRYPOINT ["dotnet", "WebApp.dll"]