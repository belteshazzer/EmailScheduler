# Email Scheduler

The **Email Scheduler** is a web application built with ASP.NET Core that allows users to schedule and send emails using the Gmail API. It supports Google OAuth authentication, email scheduling, and email management (inbox and sent emails). The application is designed to integrate seamlessly with a front-end client.

---

## **Features**
- **Google OAuth Authentication**: Securely authenticate users using their Google accounts.
- **Email Scheduling**: Schedule emails to be sent at a specific time.
- **Email Sending**: Send emails immediately or based on a schedule.
- **Inbox Management**: Fetch and display the user's Gmail inbox emails.
- **Sent Emails Management**: Fetch and display the user's sent emails.
- **Token Management**: Automatically refresh expired access tokens using refresh tokens.

---

## **Technologies Used**
- **Backend**: ASP.NET Core 8.0
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: Google OAuth 2.0
- **Task Scheduling**: Hangfire
- **Email API**: Gmail API
- **Dependency Injection**: Built-in ASP.NET Core DI
- **Logging**: Microsoft.Extensions.Logging
- **Swagger**: API documentation and testing

---

## **Project Structure**
### **Folders**
- **Controllers**: Contains API controllers for handling HTTP requests.
  - `AuthController.cs`: Handles Google OAuth authentication.
  - `MailController.cs`: Manages email sending and retrieval (inbox and sent emails).
  - `ScheduleController.cs`: Handles email scheduling.
- **Services**: Contains business logic and service classes.
  - `AccountServices`: Manages user accounts and Google authentication.
  - `EmailService`: Handles email sending, scheduling, and Gmail API integration.
- **Models**: Contains data transfer objects (DTOs) and entity models.
- **Repositories**: Implements generic repository patterns for database operations.
- **Context**: Contains the database context for Entity Framework Core.
- **Mapper**: Contains AutoMapper profiles for mapping DTOs to entities.

---

## **Setup Instructions**
### **Prerequisites**
1. .NET 8.0 SDK
2. Microsoft SQL Server
3. Google Cloud Console account with Gmail API enabled.

### **Steps**
1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd EmailScheduler
