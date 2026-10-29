# Student Food Parcel Management System (FPMS)

A web-based application for managing student food parcel applications, funding verification, approval decisions, and food parcel distribution at Walter Sisulu University (WSU).

**Built with C#, ASP.NET Core MVC, Entity Framework Core, and Microsoft SQL Server.**

[View Repository](https://github.com/SIYABONGA145/StudentFoodParcelApp)

---

## Table of Contents

- [Overview](#overview)
- [Problem Statement](#problem-statement)
- [Project Objectives](#project-objectives)
- [Key Features](#key-features)
- [User Roles](#user-roles)
- [Application Workflow](#application-workflow)
- [Technology Stack](#technology-stack)
- [Database Design](#database-design)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Screenshots](#screenshots)
- [Security Considerations](#security-considerations)
- [Project Status](#project-status)
- [Skills Demonstrated](#skills-demonstrated)
- [Contributors](#contributors)
- [Academic Context](#academic-context)

---

## Overview

The Student Food Parcel Management System (FPMS) is a web application designed to support the management of student food assistance programmes.

The system provides a centralized approach to handling food parcel applications, verifying student funding information, recording application decisions, and managing distribution and collection activities.

The project demonstrates the use of ASP.NET Core MVC and a relational SQL Server database to support a multi-step administrative workflow.

## Problem Statement

Manual food parcel application processes can rely on online forms, messaging platforms and shared lists. These processes may create delays, make it difficult for students to track their applications, and expose sensitive student information.

FPMS aims to improve this process by centralizing application management and providing the relevant users with access to the information needed for their responsibilities.

## Project Objectives

- Provide a centralized platform for student food parcel applications.
- Support verification of student registration and funding status.
- Enable authorized SRC personnel to review applications and record decisions.
- Organize food parcel cycles and associated items.
- Support distribution and collection tracking.
- Improve application visibility while protecting student information.

## Key Features

### Student Application Management
- Manage food parcel application records.
- Associate applications with students and relevant food parcel cycles.
- Support application status tracking.

### Funding Verification
- Support the Financial Aid and Bursaries verification workflow.
- Record funding verification information associated with student applications.
- Help distinguish funded and unfunded students.

### Application Review and Decisions
- Provide SRC personnel with application information for review.
- Support recording application decisions and their associated statuses.
- Maintain relationships between applications, students and decision records.

### Food Parcel Cycle Management
- Organize applications around food parcel cycles.
- Associate food parcel items with the relevant cycle.
- Support administration of the food parcel process.

### Distribution and Collection Management
- Maintain distribution-related records.
- Link application requests to distribution events where applicable.
- Support tracking of food parcel collection activities.

> Features should be considered available only where the corresponding functionality is implemented in the current version of the application.

## User Roles

The system is designed around different responsibilities within the food parcel management process.

| User role | Responsibility |
|---|---|
| Student | Submit a food parcel application and view relevant application information. |
| Financial Aid and Bursaries | Verify student registration and funding information. |
| SRC Administrator/Coordinator | Review applications, record decisions, manage food parcel cycles and coordinate distributions and collections. |

Access to specific pages and actions depends on the application's implemented authentication and authorization rules.

## Application Workflow

The intended workflow follows these main stages:

1. **Application:** A student submits an application for an available food parcel cycle.
2. **Verification:** Financial Aid and Bursaries verify the student's relevant registration and funding information.
3. **Review:** The SRC reviews the application and the verification outcome.
4. **Decision:** The application decision and corresponding status are recorded.
5. **Distribution:** Approved applications can be associated with the relevant distribution activities.
6. **Collection:** Collection information is maintained to support parcel handover tracking.

The workflow is intended to reduce reliance on manual coordination and make application handling more organized.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Application programming language |
| ASP.NET Core MVC (.NET 9) | Web application framework |
| Entity Framework Core | Object-relational mapping and database access |
| Microsoft SQL Server | Relational database management |
| SQL Server Management Studio (SSMS) | Database administration and SQL development |
| Razor Views | Server-rendered user interface |
| HTML, CSS and JavaScript | Web interface and client-side functionality |
| Visual Studio | Development environment |

The application uses cookie-based authentication, configured through ASP.NET Core's authentication middleware.

## Database Design

The application uses a relational database to organize information about students, applications, funding verification and food parcel distribution.

Key database entities include:

- Universities, campuses and departments
- Persons, students and staff records
- Food parcel cycles and cycle items
- Requests and application statuses
- Funding statuses
- Distributions and distribution-request relationships
- Notifications, venues and collection-related records

### Relationships and Data Integrity

The database uses primary keys to identify records and foreign keys to connect related entities.

For example:

- A student can have application requests associated with different food parcel cycles.
- A food parcel cycle can be associated with multiple requests and food parcel items.
- Application status records are associated with the relevant requests and decision-making records.
- Distribution records can be linked to requests through a junction table where applicable.

A unique student-and-cycle constraint helps prevent duplicate applications for the same cycle.

### Entity Relationship Diagram

Add your exported ERD image to the repository and display it here.

Example location: `docs/FPMS-ERD.png`

```markdown
![Food Parcel Management System ERD](docs/FPMS-ERD.png)
```

## Project Structure

The repository contains the main ASP.NET Core MVC application components.

```text
StudentFoodParcelApp/
├── Controllers/       # MVC controllers and request handling
├── Models/            # Data models and database context
├── Properties/        # Application launch settings
├── Views/             # Razor views and user interface pages
├── wwwroot/           # Static assets such as CSS, JS and images
├── Program.cs         # Application configuration and middleware
├── StudentFoodParcelApp.csproj
├── StudentFoodParcelApp.slnx
└── README.md
```

The structure shown above reflects the main folders and files in the repository. Additional folders can be documented as the project grows.

## Getting Started

### Prerequisites

Install the following tools:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio](https://visualstudio.microsoft.com/) with ASP.NET and web development support
- [Microsoft SQL Server](https://www.microsoft.com/sql-server)
- [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)

### 1. Clone the Repository

Open a terminal and run:

```bash
git clone https://github.com/SIYABONGA145/StudentFoodParcelApp.git
cd StudentFoodParcelApp
```

### 2. Open the Project

Open `StudentFoodParcelApp.slnx` in a compatible version of Visual Studio, or open the project directory and restore its dependencies.

### 3. Configure SQL Server

Ensure that SQL Server is running and that the project database has been created with the required tables, relationships and constraints.

Configure the `DefaultConnection` connection string in your local configuration.

The connection string must point to your own SQL Server instance and database. Do not copy machine-specific connection settings from another developer's computer.

If you are setting up the application from a fresh database, use the project's database creation scripts or documented schema setup procedure. Include those scripts in the repository so another developer can reproduce the database.

### 4. Restore Dependencies

From the project directory, run:

```bash
dotnet restore
```

### 5. Build the Application

```bash
dotnet build
```

Resolve any build errors before proceeding.

### 6. Run the Application

```bash
dotnet run
```

Alternatively, run the project using Visual Studio.

The application is configured to use the account login page as its default route. The local URL and port depend on the launch profile and application configuration.

### Database Migrations

If the project contains Entity Framework Core migrations, apply the appropriate migrations using the documented migration procedure.

Do not run migration commands blindly against a database that already contains project data. Confirm the database setup and migration history first.

## Configuration

The application reads its database connection string using the `DefaultConnection` configuration key.

A local configuration entry typically follows this structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_LOCAL_SQL_SERVER_CONNECTION_STRING"
  }
}
```

Replace the placeholder with a valid connection string for your environment.

**Security:** Never commit passwords, production connection strings, API keys or other secrets to the repository. Keep sensitive configuration in local development settings or an appropriate secrets manager.

## Screenshots

Screenshots make it easier for reviewers and recruiters to understand the application without setting it up locally.

Add genuine screenshots of the implemented pages, such as:

| Screenshot | Suggested image |
|---|---|
| SRC Admin Dashboard | `docs/screenshots/src-dashboard.png` |
| Applications Management | `docs/screenshots/applications.png` |
| Funding Verification | `docs/screenshots/funding-verification.png` |
| Application Review | `docs/screenshots/application-review.png` |
| Distribution and Collection | `docs/screenshots/distribution-collection.png` |

Once the image files have been added, display them using Markdown:

```markdown
![SRC Admin Dashboard](docs/screenshots/src-dashboard.png)
```

Only include screenshots that correspond to features available in the current application.

## Security Considerations

The application configures cookie-based authentication and uses ASP.NET Core middleware for authentication and authorization.

The project also configures antiforgery support and HTTPS redirection.

When deploying or extending the application:

- Apply authorization checks to protected controllers and actions.
- Validate user input on the server.
- Protect student and funding information from unauthorized access.
- Keep credentials and secrets out of source control.
- Use HTTPS in deployed environments.
- Test access permissions and application workflows before release.

These are security considerations and configuration details, not a claim that the application has undergone a formal security audit.

## Project Status

**Development status:** Update this section to reflect the current state of the application, such as In Development or Completed Academic Project.

Further improvements can be documented as they are planned and implemented.

## Skills Demonstrated

This project provides practical experience in:

- C# application development
- ASP.NET Core MVC architecture
- Entity Framework Core and relational database access
- SQL Server database design and relationships
- CRUD operations and application workflow management
- Cookie-based authentication
- Server-side validation and web application security
- Git and GitHub version control
- Technical documentation

Only claim skills and functionality that you can demonstrate in the code or during a project walkthrough.

## Contributors

Add the names of the team members who contributed to the project.

- **Project contributor:** Siyabonga Maseko
- **Additional contributors:** Add team members where applicable.

## Academic Context

Developed as an academic software development project at Walter Sisulu University.

The project applies software development and database design principles to a practical student food assistance management problem.

## License

Add a license if the project is intended to be distributed or reused under specific terms. If no license has been selected, state that the project is currently provided without an explicit open-source license.
