# Loan API - სესხების მართვის სისტემა

## 📋 პროექტის აღწერა

Loan API არის RESTful Web API სესხების მართვისთვის. სისტემა მხარს უჭერს ორ როლს: რიგითი მომხმარებლები (User) და ბუღალტრები (Accountant), რომლებსაც აქვთ სხვადასხვა უფლებები სესხების მართვაზე.

## 🚀 გამოყენებული ტექნოლოგიები

- **ASP.NET Core 8.0** - Web API Framework
- **Entity Framework Core 8.0** - ORM
- **SQL Server** - Database
- **JWT (JSON Web Tokens)** - Authentication
- **BCrypt.Net** - Password Hashing
- **FluentValidation** - Input Validation
- **Serilog** - Logging
- **Swagger/OpenAPI** - API Documentation
- **xUnit** - Unit Testing Framework
- **Moq** - Mocking Framework
- **FluentAssertions** - Test Assertions

## 🏗️ არქიტექტურა

პროექტი აგებულია Clean Architecture პრინციპების მიხედვით:
```
Controllers/     - API Endpoints (HTTP Request/Response)
Services/        - Business Logic
Repositories/    - Data Access Layer
Models/          - Entities & DTOs
Validators/      - FluentValidation Rules
Middleware/      - Exception Handling
Helpers/         - JWT Token Generation
```

### SOLID პრინციპები
- ✅ Single Responsibility
- ✅ Open/Closed
- ✅ Liskov Substitution
- ✅ Interface Segregation
- ✅ Dependency Inversion

## 📊 Database Structure

### Users Table
- Id, FirstName, LastName, Username (unique)
- Age, Email (unique), MonthlyIncome
- IsBlocked (default: false)
- PasswordHash (BCrypt)
- Role ("User" or "Accountant")

### Loans Table
- Id, UserId (FK to Users)
- LoanType (1=QuickLoan, 2=AutoLoan, 3=Installment)
- Amount, Currency (1=GEL, 2=USD, 3=EUR)
- Period (months)
- Status (1=InProcess, 2=Approved, 3=Rejected)
- CreatedAt, UpdatedAt

**Relationship:** One User → Many Loans

## 🔧 როგორ გავუშვათ

### წინაპირობები
- .NET 8.0 SDK
- SQL Server Express ან LocalDB
- Visual Studio 2022

### ინსტალაცია

1. Clone/Download პროექტი
2. Visual Studio-ში გახსენით `.sln` ფაილი
3. `appsettings.json`-ში შეამოწმეთ connection string
4. Package Manager Console-ში:
```
Update-Database
```
5. დააჭირეთ **F5** პროექტის გასაშვებად

პროექტი გაიხსნება Swagger UI-ში: `https://localhost:7058/`

## 🔐 Default Accountant
```
Username: accountant
Password: Admin123!
```

## 📚 API Endpoints

### Authentication (არ საჭიროებს token-ს)

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "გიორგი",
  "lastName": "მელაძე",
  "username": "gmeladze",
  "age": 25,
  "email": "giorgi@test.com",
  "monthlyIncome": 2500,
  "password": "Password123"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "gmeladze",
  "password": "Password123"
}
```

### User Endpoints (საჭიროებს JWT token)

- `GET /api/user/me` - მიმდინარე მომხმარებელი
- `GET /api/user/{id}` - მომხმარებელი ID-ით

### Loan Endpoints (User Role)

#### Create Loan
```http
POST /api/loan
Authorization: Bearer {token}
Content-Type: application/json

{
  "loanType": 1,
  "amount": 5000,
  "currency": 1,
  "period": 12
}
```

- `GET /api/loan` - ჩემი სესხები
- `GET /api/loan/{id}` - კონკრეტული სესხი
- `PUT /api/loan/{id}` - განახლება (მხოლოდ InProcess)
- `DELETE /api/loan/{id}` - წაშლა (მხოლოდ InProcess)

### Accountant Endpoints (Accountant Role)

- `GET /api/accountant/loans` - ყველა სესხი
- `GET /api/accountant/loans/{id}` - ნებისმიერი სესხი
- `PUT /api/accountant/loans/{id}` - სესხის განახლება
- `PATCH /api/accountant/loans/{id}/status` - სტატუსის შეცვლა
- `DELETE /api/accountant/loans/{id}` - სესხის წაშლა
- `PUT /api/accountant/users/{id}/block` - მომხმარებლის დაბლოკვა
- `PUT /api/accountant/users/{id}/unblock` - განბლოკვა

## 🧪 ტესტირება

პროექტი შეიცავს **52 unit და integration tests**:
```
Test → Run All Tests
```

ან Visual Studio-ში:
```
Ctrl + R, A
```

### Test Categories
- **Repository Tests (18)** - Database operations
- **Service Tests (16)** - Business logic
- **Validation Tests (28)** - Input validation
- **Integration Tests (7)** - Complete workflows

## 🔒 უსაფრთხოება

- **JWT Authentication** - Token-based auth
- **Role-Based Authorization** - User vs Accountant
- **Password Hashing** - BCrypt algorithm
- **Input Validation** - FluentValidation
- **Exception Handling** - Custom middleware
- **Logging** - Serilog to files

## 📝 მთავარი Features

✅ მომხმარებლის რეგისტრაცია და ავტორიზაცია
✅ JWT token-ებით authentication
✅ Role-based წვდომის კონტროლი
✅ სესხების CRUD ოპერაციები
✅ მომხმარებლის დაბლოკვა/განბლოკვა
✅ სტატუსის მართვა (Accountant-ის მიერ)
✅ ვალიდაცია ყველა input-ზე
✅ Exception handling middleware
✅ Comprehensive logging
✅ 69 automated tests

## 🎯 ბიზნეს წესები

1. **მომხმარებელს შეუძლია:**
   - საკუთარი სესხების ნახვა
   - სესხის შექმნა (თუ არ არის დაბლოკილი)
   - სესხის განახლება/წაშლა (მხოლოდ InProcess სტატუსზე)

2. **ბუღალტერს შეუძლია:**
   - ყველა სესხის ნახვა
   - ნებისმიერი სესხის განახლება/წაშლა
   - სესხის სტატუსის შეცვლა
   - მომხმარებლის დაბლოკვა

3. **დაბლოკილ მომხმარებელს არ შეუძლია სესხის შექმნა**

4. **სტატუსის შეცვლა მხოლოდ Accountant-ს შეუძლია**

## 👨‍💻 ავტორი

სერგი არაბიძე
sergiarabidze15@gmail.com