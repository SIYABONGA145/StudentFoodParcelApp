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
* **Database:** [Insert your database here, e.g., SQL Server / PostgreSQL / SQLite]
* **Authentication:** ASP.NET Core Identity

## 📋 Prerequisites

Before running this project, ensure you have the following installed on your machine:

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Entity Framework Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) 
  * Install via terminal: `dotnet tool install --global dotnet-ef`
* [Insert your database name, e.g., Microsoft SQL Server] running locally or remotely.

## 💻 How to Run This Project Locally

Follow these steps to get the application running on your local machine.

**1. Clone the repository:**
```bash
git clone https://github.com/siyabonga145/student-food-parcel-app.git
cd student-food-parcel-app
