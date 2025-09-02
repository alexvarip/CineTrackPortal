# CineTrack Portal

An ASP.NET Core MVC (Razor Views) web-based application for cinema records management and movie tracking. Built with ASP.NET Core (.NET 9) and Entity Framework Core.

## Features

- **Movie Management:** List, search, create, edit, and delete movies with actor association.
- **Actor Management:** Add, view, and manage actor records with associated movies.
- **User Management:** Add, view, and manage movie records.
- **Dashboard:** Navigation links to movie, actor and user management.
- **Historical Info:** View basic historical data each movie record.
- **Interactive UI:** Movie, Actor and User selection, management, and alert handling using partial views for dynamic content updates.
- **Responsive Design:** Modern, responsive, mobile-friendly layout using Bootstrap. Built with Razor Views for fast and secure web experiences.
- **Authentication:** User authentication via ASP.NET Core Identity with secure login, registration, and account management.
- **Security:** Authorization required for all main features and anti-forgery token validation on forms.

## Technology Stack

- ASP.NET Core (.NET 9) MVC (Razor Views)
- Entity Framework Core (MS SQL Server)
- Bootstrap for responsive UI

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB or compatible instance)

### Setup

1. **Clone the repository:**

2. **Configure the database:**
   - Update the connection string in `appsettings.json` if needed.

3. **Apply migrations:**
   - dotnet ef database update

4. **Run the application:**	
   - dotnet run

5. **Access the app:**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5288`.

> **Note:** 
> - If you restore the database from a SQL backup file, you usually do not need to apply migrations unless new migrations have been added since the backup was created. 
> - If in doubt, you can run `dotnet ef database update` (#Database Migration Setup) to ensure the database schema is current.
	
## Restoring the Database from a SQL Backup

If you have a SQL backup file (`.sql` or `.bak`), follow these steps to restore the database:

1. **For `.bak` files (SQL Server):**
   - Open SQL Server Management Studio (SSMS).
   - Connect to your SQL Server instance.
   - Right-click on the `Databases` node and select `Restore Database...`.
   - Choose "Device", browse to select your `.bak` file, and follow the prompts to restore.

2. **For `.sql` files:**
   - Open SQL Server Management Studio (SSMS) or your preferred SQL client.
   - Connect to your SQL Server instance.
   - Open the `.sql` file and execute it to recreate the database schema and data.

3. **Update the connection string** in `appsettings.json` to match your restored database.

> **Note:** Ensure your SQL Server instance is running and you have the necessary permissions to restore databases.

## Architecture Overview

- **Models:** `Movie`, `Actor` and `User` entities with proper relationships
- **Dashboard API Endpoints:**
  - `GET /Dashboard`
  -	`GET /About`
  
- **Movie API Endpoints:**
  - `GET /Movies`
  -	`GET /Movies/ListMovies`
  -	`GET /Movies/Details/{Guid}`
  -	`GET /Movies/Create`
  -	`POST /Movies/Create`
  -	`GET /Movies/Edit/{Guid}`
  -	`POST /Movies/Edit/{Guid}`
  -	`GET /Movies/Delete/{Guid}`
  -	`POST /Movies/Delete/{Guid}`

- **Actor API Endpoints:**
  - `GET /Actors`
  -	`GET /Actors/ListActors`
  -	`GET /Actors/Details/{Guid}`
  -	`GET /Actors/Create`
  -	`POST /Actors/Create`
  -	`GET /Actors/Edit/{Guid}`
  -	`POST /Actors/Edit/{Guid}`
  -	`GET /Actors/Delete/{Guid}`
  -	`POST /Actors/Delete/{Guid}`  
  
- **User API Endpoints:**
  - `GET /UserManagement`
  -	`GET /UserManagement/ListUsers`
  -	`GET /UserManagement/Details/{Guid}`
  -	`GET /UserManagement/Create`
  -	`POST /UserManagement/Create`
  -	`GET /UserManagement/Edit/{Guid}`
  -	`POST /UserManagement/Edit/{Guid}`
  -	`GET /UserManagement/Delete/{Guid}`
  -	`POST /UserManagement/Delete/{Guid}`  

## Project Structure

- `/Areas/Identity/Pages/`                # ASP.NET Core Identity (auth/account management)
- `/Controllers/`                         # MVC controllers (e.g., MoviesController.cs)
- `/Data/ApplicationDbContext.cs`         # Entity Framework Core DbContext
- `/Data/Migrations/`                     # EF Core migrations
- `/Models/`                              # Entity and ViewModel classes (e.g., MovieModel.cs, ActorModel.cs)
- `/Services/`							  # Email service class and interface
- `/Views/Shared/`                        # Shared Razor views and layouts (e.g., _Layout.cshtml)
- `/Views/Dashboard/`                     # Dashboard controller views (e.g., Index.cshtml)
- `/Views/Movies/`                        # Movies controller views (e.g., Index.cshtml, Create.cshtml, Edit.cshtml, Details.cshtml)
- `/Views/Actors/`                        # Actors controller views (e.g., Index.cshtml, Create.cshtml, Edit.cshtml, Details.cshtml)
- `/Views/UserManagement/`                # UserManagement controller views (e.g., Index.cshtml, Create.cshtml, Edit.cshtml, Details.cshtml)
- `/wwwroot/`                             # Static files (CSS, JS, images)

### Database Migration Setup

1. **Install EF Core Tools (if not already installed):**

2. **Apply Migrations to the Database:**
   - dotnet ef database update

3. **Verify the Database:**
   - Ensure your connection string in `appsettings.json` is correct for your environment (SQLite or SQL Server).
   - The database will be created and seeded with sample data if configured.
   
> **Note:**  
> - Run these commands from the project directory containing the `.csproj` file.
> - For SQLite, ensure the database file path is accessible and writable.

#### Data Seeding Details

- **Migration-based seeding:**  
  The initial database migration seeds two actors (Tom Hanks, Meryl Streep), two movies ("Forrest Gump", "The Post"), and their relationships. This ensures the database always has basic demo data after migration.

- **CSV-based runtime seeding:**  
  The project includes a `CsvDataSeeder` utility that can import additional movies and actors from a CSV file (e.g., `imdb_movies_mini.csv`). This allows bulk import of movie and actor data at runtime. The seeder:
  - Reads each row for movie title, date, and actor names.
  - Avoids duplicates by checking for existing movies.
  - Adds new actors if they do not exist.
  - Associates actors with movies and saves all changes.

## Initial Sample Data

- **Movies/Actors**
  - Forrest Gump (1994), Tom Hanks
  - The Post (2017), Tom Hanks, Meryl Streep

## API Documentation

- See [API Endpoints](#architecture-overview) for available routes and usage.

## License

This project is licensed under the MIT License.