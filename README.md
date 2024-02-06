# Cosmos Odyssey

## To run the project
1. Make sure you have docker installed on your machine.
2. Clone the repository.
3. Run `docker-compose up --build` in the root directory of the project.
4. The project will be available on `http://localhost:8000`.

#### If you want to run the project without docker
1. Make sure you have a running PostgreSQL database and .NET 8.
2. Clone the repository.
3. Change the connection string in the `WebApp/appsettings.json` file in the root directory of the project.
4. Run `dotnet run --project WebApp` in the root directory of the project.
5. The project will be available on `http://localhost:5225`.