# FitApp
:runner::fire::fire::fire::fire::fire::fire: <br />
 Group project - web application development and testing
# Project Setup Instructions

## Pulling and Running the Project on a New Computer

When pulling the project repository to a new computer, follow these steps to ensure that all functions, including registration, login, and CRUD operations, work correctly.

### Version Control (Git) Setup

- Before pulling the repository to another computer, make sure that all files are committed and pushed to your remote repository. This includes:
  - `Controllers`
  - `Views`
  - `Models`
  - `wwwroot` (CSS, JavaScript, images)
  - `appsettings.json`
  - `Program.cs` and `Startup.cs` (if using .NET Core 3.x or earlier)
  - `.gitignore` (to avoid committing unnecessary files)

### Pulling the Repository

- On the new computer, clone the repository or pull the latest changes from the remote repository. :point_down:

  ```bash
  git clone <repository-url>

### Restore Dependencies

- Ensure that the .NET SDK version used in your project is installed on the new computer. You can check the required SDK version in your `.csproj` file.
  
- Run the following command to restore NuGet packages that your project depends on: :point_down:

  ```bash
  dotnet restore

### Database Configuration

- Check the connection string in `appsettings.json`. Ensure it points to the correct database on the new machine. :point_down:

  ```json
  "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FitAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
- Update the database to local machine: :point_down:

  ```bash
  Database-Update
### Testing the Application

- Start the application and test all major functionalities:
  
  - Registration and Login
  - CRUD operations for workouts
  - Navigation between pages
  - Database interactions :point_down:

  ```bash
  dotnet run

### For Entity Framework Core

- Running migrations to make sure database is up-to-date. :point_down:
  ```bash
  dotnet ef database update
## Class Diagram
![](ClassDiagram/Fithub-UMLClassDiagram.jpg)


