# Misk Assessment API

## Test Accounts

### Manager Role

```json
{
  "email": "Manager@gmail.com",
  "password": "Manager@123"
}
```

### Admin Role

```json
{
  "email": "admin@gmail.com",
  "password": "Admin@123"
}
```

### Learner Account

```json
{
  "email": "3laa.m0o0barak@gmail.com",
  "password": "3laa@Mobarak"
}
```

---

# Running the Project

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Set the Web API project as the startup project.
4. Run the solution.
5. Use **Postman** or **ApiDog** to test the APIs.

> **Note:** I encountered an issue with Swagger when testing some `GetById`, `Update`, and `Delete` endpoints. These endpoints work correctly when tested using Postman or ApiDog.

---

# Database Setup

No database setup is required.

The database is already hosted and accessible through MonsterASP.NET, so the application can be run directly after cloning the repository.

---

# Architecture

The solution follows a layered architecture to separate concerns and improve maintainability.

```text
mskAssessment (Solution)
│
├── Domain Layer
│   └── Contains Entities, Enums, and Repository Interfaces.
│
├── Application Layer
│   └── Contains Services, Service Contracts (Interfaces),
│       and JWT Helper utilities.
│
├── DTO Layer
│   └── Contains Request/Response DTOs and Validation Rules.
│
├── Infrastructure Layer
│   └── Contains DbContext, Entity Configurations,
│       Repositories, and Unit of Work implementation.
│
└── Presentation Layer (Web API)
    └── Contains Controllers, Middleware,
        Authentication Configuration, and Program.cs.
```

---

# Design Decisions & Assumptions

### Authentication

The assessment requirement mentioned:

> "Don't implement full authentication."

I was unsure whether this meant skipping authentication entirely or implementing a simplified version. Therefore, I implemented a complete JWT-based authentication flow including:

* User Registration
* Login
* JWT Token Generation
* Role-based Authorization

### Learner Entity

I treated Learners as a separate domain entity/table instead of using the default Identity User as the learner model.

### Enrollment Relationship

The relationship between Learners and Courses was considered **Many-to-Many**.

A junction table named **Enrollment** was created.

Although a composite key of:

```text
LearnerId + CourseId
```

would be sufficient, I introduced a dedicated primary key:

```text
EnrollmentId
```

for easier management and future extensibility.

---

# API Endpoints

## Authentication

### Register

```http
POST /api/register
```

Example:

```http
http://localhost:5160/api/register
```

### Login / Generate Token

```http
POST /api/token
```

Example:

```http
http://localhost:5160/api/token
```

---

# Course Endpoints

### Get Course By Id

```http
GET /api/Course/{id}
```

Example:

```http
http://localhost:5160/api/Course/1
```

> Use Postman or ApiDog.

### Get All Courses

```http
GET /api/Course?skip=0&take=10
```

Example:

```http
http://localhost:5160/api/Course?skip=0&take=10
```

### Create Course

```http
POST /api/Course
```

### Update Course

```http
PUT /api/Course/{id}
```

Example:

```http
http://localhost:5160/api/Course/1
```

> Use Postman or ApiDog.

### Delete Course

```http
DELETE /api/Course/{id}
```

Example:

```http
http://localhost:5160/api/Course/1
```

> Use Postman or ApiDog.

---

# Enrollment Endpoints

### Submit Enrollment

```http
POST /api/Enrollment/submit
```

### Review Enrollment

```http
PUT /api/Enrollment/review
```

### Get Enrollments By Learner Id

```http
GET /api/Enrollment/learner/{id}
```

Example:

```http
http://localhost:5160/api/Enrollment/learner/1
```

> Use Postman or ApiDog.

### Get Pending Enrollments

```http
GET /api/Enrollment/pending
```

Example:

```http
http://localhost:5160/api/Enrollment/pending
```

### Search Enrollments

Supports filtering by:

* LearnerId
* CourseId
* Status
* FromDate
* ToDate

Example:

```http
GET /api/Enrollment?learnerId=1&courseId=1&status=1&fromDate=2025-01-01&toDate=2025-12-31
```

---

# Learner Endpoints

### Get All Learners

```http
GET /api/Learner?skip=0&take=10
```

Example:

```http
http://localhost:5160/api/Learner?skip=0&take=10
```

### Create Learner

```http
POST /api/Learner
```

### Get Learner By Id

```http
GET /api/Learner/{id}
```

Example:

```http
http://localhost:5160/api/Learner/1
```

> Use Postman or ApiDog.

---

# Technologies Used

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Repository Pattern
* Unit of Work Pattern
* Fluent Validation
* Swagger/OpenAPI
* ASP.NET Core Identity
