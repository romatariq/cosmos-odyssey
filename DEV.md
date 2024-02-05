# create migration
~~~bash
dotnet ef migrations add DuplicateSchema --project DAL --startup-project WebApp --context AppDbContext
dotnet ef database update --project DAL --startup-project WebApp --context AppDbContext
~~~

# Scaffold razor pages
~~~bash
cd WebApp
dotnet aspnet-codegenerator razorpage -m Provider -dc AppDbContext -udl -outDir Pages/Providers –referenceScriptLibraries
~~~