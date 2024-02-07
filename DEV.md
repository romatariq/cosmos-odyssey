# create migration
~~~bash
dotnet ef migrations add ReservationNames --project DAL --startup-project WebApp --context AppDbContext
dotnet ef database update --project DAL --startup-project WebApp --context AppDbContext
~~~

# Scaffold razor pages
~~~bash
cd WebApp
dotnet aspnet-codegenerator razorpage -m Domain.Reservation -dc AppDbContext -udl -outDir Pages/Reservations –referenceScriptLibraries
~~~

# Docker
~~~bash
# --build updates the image
docker-compose up --build

# Docker Hub
docker build -t cosmos-odyssey .
docker tag cosmos-odyssey romatariq/cosmos-odyssey:latest
docker push romatariq/cosmos-odyssey:latest
~~~