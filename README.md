# Student Food Parcel App

A robust web application built with **ASP.NET Core MVC (.NET 9)** and **Entity Framework Core** to streamline the management of student food parcel applications, funding verification, and distribution.

## 📖 Overview

The Student Food Parcel App is designed to assist institutions in managing food security programs. It provides a seamless workflow for students to apply for parcels, finance teams to verify funding, and administrators to approve and distribute parcels using QR code technology. 

## 🚀 Features

* **Role-based Authentication:** Secure access tailored for Students, Admins, Finance, and IT staff.
* **Student Portal:** Allows students to apply for food parcels and track their application status.
* **Admin Dashboard:** Centralized hub for reviewing, approving, and managing applications.
* **Funding Verification:** Dedicated tools for the Finance team to verify and allocate funding.
* **QR Code Generation:** Generates unique QR codes for students to use during parcel collection, ensuring secure and trackable distribution.

## 🛠️ Tech Stack

* **Framework:** ASP.NET Core MVC (.NET 9)
* **ORM:** Entity Framework Core
* **Database:** Microsoft SQL Server
* **Authentication:** ASP.NET Core Identity

## 📋 Prerequisites

Before running this project, ensure you have the following installed on your machine:

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Entity Framework Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) 
  * Install via terminal: `dotnet tool install --global dotnet-ef`
* Microsoft SQL Server (and SQL Server Management Studio) running locally or remotely.

## 💻 How to Run This Project Locally

Follow these steps to get the application running on your local machine.

**1. Clone the repository:**
```bash
git clone https://github.com/siyabonga145/student-food-parcel-app.git
cd student-food-parcel-app
2. Set your database connection string using the .NET Secret Manager:

bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Your_Connection_String_Here"
3. Apply database migrations:
This command will create the database schema.

bash
dotnet ef database update
4. Run the application:

bash
dotnet run
Once the application is running, open your browser and navigate to the local URL provided in the terminal (typically https://localhost:5001 or http://localhost:5000).

🔐 Default Login Credentials
The database includes seed data for testing the different user roles. Use the following accounts to log in:

Role	Email	Password
Admin	admin@example.com	Admin123!
Student	student@example.com	Student123!
Finance	finance@example.com	Finance123!
IT	it@example.com	IT123!
📁 Project Structure
text
├── Controllers/      # Handles HTTP requests and routing
├── Models/           # Database entities and ViewModels
├── Views/            # Razor view templates (.cshtml)
├── Data/             # DbContext, migrations, and seed data
├── Services/         # Business logic (e.g., QR code generation)
├── wwwroot/          # Static files (CSS, JS, images)
└── Program.cs        # Application entry point and service configuration
📄 License
This project is licensed under the MIT License.


