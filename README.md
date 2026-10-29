# Student Food Parcel App

A web application built with ASP.NET Core MVC (.NET 9) and Entity Framework Core to manage student food parcel applications, funding verification, and distribution.

## 🚀 Features
* Role-based authentication (Student, Admin, Finance, IT)
* Apply for food parcels
* Admin dashboard for application approval
* Funding verification
* QR Code generation for parcel collection

## ⚙️ How to Run This Project Locally
1. Clone the repository.
2. Set your database connection string using .NET Secret Manager:
   `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Your_Connection_String_Here"`
3. Apply database migrations:
   `dotnet ef database update`
4. Run the application:
   `dotnet run`
