# Solution Documentation

**Candidate Name:** Ganapati Bhat  
**Completion Date:** 12/01/2026

---

## Problems Identified

_Describe the issues you found in the original implementation. Consider aspects like:_
- Architecture and design patterns
- Code quality and maintainability
- Security vulnerabilities
- Performance concerns
- Testing gaps


### Critical Security Issues
1. **SQL Injection Vulnerabilities** - All database operations used string concatenation
2. **Information Disclosure** - Raw exception messages exposed to clients
3. **No Input Validation** - Missing model validation and sanitization

### Architectural Problems
1. **No Dependency Injection** - Direct service instantiation in controllers
2. **Poor API Design** - Using POST for all operations instead of REST conventions
3. **Tight Coupling** - Hard-coded connection strings and dependencies
4. **No Error Handling** - Generic exception catching without proper logging

### Code Quality Issues
1. **Poor Testability** - Direct instantiation made unit testing difficult
2. **No Configuration Management** - Hard-coded values throughout
3. **Missing Logging** - No structured logging infrastructure

---

## Architectural Decisions

_Explain the architecture you chose and why. Consider:_
- Design patterns applied
- Project structure changes
- Technology choices
- Separation of concerns

### Repository Pattern
- **Why**: Abstracts data access, enables dependency injection, improves testability
- **Implementation**: ITodoRepository interface with TodoRepository implementation

### Parameterized Queries
- **Why**: Prevents SQL injection attacks
- **Implementation**: All database operations use SqliteParameter objects
- **EFCore**: For further improvement we can incorporate EFCore which by defaut SQL injection resistance

---

## Trade-offs

_Discuss compromises you made and the reasoning behind them. Consider:_
- What did you prioritize?
- What did you defer or simplify?
- What alternatives did you consider?

[Your trade-offs here]

---

## How to Run

### Prerequisites
- .NET 8.0 SDK
- SQLite (included with .NET)

### Build
```bash
cd TodoApi
dotnet restore
dotnet build
```

### Run
```bash
dotnet run --project TodoApi
```

### Test
```bash
dotnet test
```
dotnet test --collect:"XPlat Code Coverage"
---

## API Documentation

### Endpoints

#### Create TODO
```
Method: POST
URL: /api/todo
Request Body: 
{
  "title": "Learn .NET",
  "description": "Complete the tutorial"
}
Response: 201 Created
{
  "id": 1,
  "title": "Learn .NET",
  "description": "Complete the tutorial",
  "isCompleted": false,
  "createdAt": "2024-01-12T10:30:00Z"
}

```

#### Get TODO(s)
```
Method: GET
URL: /api/todo
Response: 200 OK
[
  {
    "id": 1,
    "title": "Learn .NET",
    "description": "Complete the tutorial",
    "isCompleted": false,
    "createdAt": "2024-01-12T10:30:00Z"
  }
]

```

#### Update TODO
```
Method: PUT
URL: /api/todo/{id}
Request Body:
{
  "title": "Learn .NET Core",
  "description": "Complete advanced tutorial",
  "isCompleted": true
}
Response: 200 OK / 404 Not Found

```

#### Delete TODO
```
Method: DELETE
URL: /api/todo/{id}
Response: 204 No Content / 404 Not Found
```

---

## Future Improvements

_What would you do if you had more time? Consider:_
- Additional features : 
- Performance optimizations
- Enhanced testing
- Better documentation
- Deployment considerations

- **Authentication & Authorization** - JWT token-based auth with role-based access
- **Todo Categories/Tags** - Organize todos by categories or tags
- **Pagination** - For GET /api/todo to handle large datasets
- **Entity Framework Core** - Replace raw SQL with EF Core for better performance
- **Caching Layer** - Redis caching for frequently accessed data
- **Database Indexing** - Add proper indexes for query optimization
- **Test Coverage** - Icreasing the both Unit and intigration testing.
- **Depoyment** - Docker can be incoporated to deploy and build the service.
