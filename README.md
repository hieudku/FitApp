# FitApp
:runner::fire::fire::fire::fire::fire::fire: <br />
 Group project - web application development and testing
# Project Setup Instructions

### Pulling the Repository

- On the new computer, clone the repository or pull the latest changes from the remote repository. :point_down:

  ```bash
  git clone https://github.com/hieudku/FitApp.git

### Restore Dependencies
  
- Run the following command to restore NuGet packages that the project depends on: :point_down:

  ```bash
  dotnet restore

### Database Configuration

- Check the connection string in `appsettings.json`. :point_down:

  ```json
  "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FitAppContext;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
- Update the database to local machine: :point_down:

  ```bash
  Update-Database
### Testing the Application

- Start the application and test all major functionalities:
  
  - Registration and Login
  - CRUD operations for workouts
  - Navigation between pages
  - Database interactions :point_down:

  ```bash
  dotnet run
or Ctrl + F5
### For Entity Framework Core

- Running migrations to make sure database is up-to-date. :point_down:
  ```bash
  dotnet ef database update
## Class Diagram



