# LMS API Testing Guide - Phase 2 Authorization

## 📋 Prerequisites

### 1. Chạy Application
```powershell
cd C:\Users\phamv\Desktop\LMS-OJT\Backend\LMS-Backend\LMS\LMS.API
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### 2. Tools Cần Thiết
- ✅ **Postman** - Test API endpoints
- ✅ **jwt.io** - Decode JWT tokens
- ✅ **pgAdmin** hoặc **DBeaver** - Verify database changes

### 3. Database State Check
Verify roles đã được seeded:
```sql
SELECT * FROM "Roles";
```

**Expected:**
| Id | Name       | CreatedAt           | UpdatedAt |
|----|------------|---------------------|-----------|
| 1  | Admin      | 2026-02-05 00:00:00 | NULL      |
| 2  | Instructor | 2026-02-05 00:00:00 | NULL      |
| 3  | Student    | 2026-02-05 00:00:00 | NULL      |

---

## 🧪 Test Case 1: Register Flow với Role Assignment

**Objective:** Verify user registration tự động assign Student role

### Step 1: Register New User

**Request:**
```http
POST https://localhost:5001/api/v1/auth/register
Content-Type: application/json

{
  "email": "student1@test.com",
  "password": "Student@123",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Expected Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "email": "student1@test.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### Step 2: Verify Database - Users Table

```sql
SELECT "Id", "Email", "FirstName", "LastName", "Status" 
FROM "Users" 
WHERE "Email" = 'student1@test.com';
```

**Expected:**
| Id (UUID)                            | Email              | FirstName | LastName | Status |
|--------------------------------------|--------------------|-----------|----------|--------|
| xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx | student1@test.com  | John      | Doe      | 0      |

✅ **Status = 0** = UserStatus.Active (enum)

### Step 3: Verify Database - UserRoles Table

```sql
SELECT ur."UserId", ur."RoleId", r."Name" 
FROM "UserRoles" ur
JOIN "Roles" r ON ur."RoleId" = r."Id"
WHERE ur."UserId" = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx';
```

**Expected:**
| UserId (UUID)                        | RoleId | Name    |
|--------------------------------------|--------|---------|
| xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx | 3      | Student |

✅ **RoleId = 3** = Student role được assign tự động

### ✅ Pass Criteria
- [ ] Response 200 OK với token
- [ ] User record tạo thành công trong `Users` table
- [ ] User.Status = 0 (Active)
- [ ] Record tạo trong `UserRoles` với RoleId = 3 (Student)

---

## 🧪 Test Case 2: Login Flow với JWT Role Claims

**Objective:** Verify JWT token chứa role claims

### Step 1: Login với User Đã Register

**Request:**
```http
POST https://localhost:5001/api/v1/auth/login
Content-Type: application/json

{
  "email": "student1@test.com",
  "password": "Student@123"
}
```

**Expected Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ4eHh4eHh4eC14eHh4LXh4eHgteHh4eC14eHh4eHh4eHh4eHgiLCJlbWFpbCI6InN0dWRlbnQxQHRlc3QuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiU3R1ZGVudCIsIm5hbWUiOiJKb2huIERvZSIsImp0aSI6Inh4eHh4eHh4LXh4eHgteHh4eC14eHh4LXh4eHh4eHh4eHh4eCIsImV4cCI6MTczODgwMDAwMCwiaXNzIjoiTE1TQVBJIiwiYXVkIjoiTE1TQ2xpZW50In0.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "user": {
    "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "email": "student1@test.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### Step 2: Decode JWT Token

1. Copy token từ response
2. Mở https://jwt.io
3. Paste token vào "Encoded" section

**Expected Decoded Payload:**
```json
{
  "sub": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "email": "student1@test.com",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Student",
  "name": "John Doe",
  "jti": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "exp": 1738800000,
  "iss": "LMSAPI",
  "aud": "LMSClient"
}
```

### ✅ Pass Criteria
- [ ] Response 200 OK với token
- [ ] JWT có claim `sub` (user ID)
- [ ] JWT có claim `email`
- [ ] JWT có claim `role` = "Student" (ClaimTypes.Role)
- [ ] JWT có claim `name` (full name)
- [ ] JWT có `exp`, `iss`, `aud` claims

---

## 🧪 Test Case 3: Authorization trên Course APIs

**Objective:** Verify `[Authorize(Roles)]` hoạt động đúng

### Setup: Check CoursesController Authorization

Verify [CoursesController.cs](LMS/LMS.API/Controllers/CoursesController.cs) có attribute:
```csharp
[HttpPost]
[Authorize(Roles = "Admin,Instructor")]
public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto dto)
```

### Test 3.1: POST Course với Student Token (Expect 403)

**Request:**
```http
POST https://localhost:5001/api/v1/courses
Authorization: Bearer <STUDENT_TOKEN_FROM_LOGIN>
Content-Type: application/json

{
  "title": "Test Course",
  "description": "This should fail",
  "instructorId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

**Expected Response:** `403 Forbidden`
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "traceId": "00-xxxxxx-xxxxxx-00"
}
```

✅ **403 = Authorization failed** - Student không có quyền tạo course

### Test 3.2: POST Course KHÔNG có Token (Expect 401)

**Request:**
```http
POST https://localhost:5001/api/v1/courses
Content-Type: application/json

{
  "title": "Test Course",
  "description": "This should fail"
}
```

**Expected Response:** `401 Unauthorized`

✅ **401 = Authentication missing** - Không có Bearer token

### Test 3.3: Create Instructor User để Test (Manual)

**Option A: SQL Insert**
```sql
-- Insert instructor user
INSERT INTO "Users" ("Id", "Email", "PasswordHash", "FirstName", "LastName", "Status", "CreatedAt")
VALUES (
  'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
  'instructor1@test.com',
  -- Hash for "Instructor@123" - cần dùng PasswordHasher để generate
  'AQAAAAIAAYagAAAAExxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx',
  'Jane',
  'Smith',
  0,
  NOW()
);

-- Assign Instructor role
INSERT INTO "UserRoles" ("UserId", "RoleId")
VALUES (
  'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
  2  -- Instructor role
);
```

**Option B: Create Endpoint (Recommended - Task 15)**

### Test 3.4: POST Course với Instructor Token (Expect 201)

**Prerequisites:**
1. Register instructor user: `instructor1@test.com`
2. Manually update user_roles: Set RoleId = 2 (Instructor)
3. Login to get Instructor token

**Request:**
```http
POST https://localhost:5001/api/v1/courses
Authorization: Bearer <INSTRUCTOR_TOKEN_FROM_LOGIN>
Content-Type: application/json

{
  "title": "Introduction to C#",
  "description": "Learn C# from scratch",
  "instructorId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  "status": 0,
  "thumbnailUrl": "https://example.com/thumb.jpg"
}
```

**Expected Response:** `201 Created`
```json
{
  "id": 1,
  "title": "Introduction to C#",
  "description": "Learn C# from scratch",
  "instructorId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  "status": 0,
  "thumbnailUrl": "https://example.com/thumb.jpg",
  "createdAt": "2026-02-05T10:30:00Z"
}
```

✅ **201 = Success** - Instructor có quyền tạo course

### ✅ Pass Criteria
- [ ] Student token → 403 Forbidden khi POST /courses
- [ ] No token → 401 Unauthorized
- [ ] Instructor token → 201 Created với course data
- [ ] Admin token → 201 Created (if tested)

---

## 🧪 Test Case 4: Verify API Versioning

**Objective:** Confirm tất cả endpoints dùng `/v1/` prefix

### Test 4.1: Auth Endpoints

```http
# ✅ Should work
POST https://localhost:5001/api/v1/auth/register
POST https://localhost:5001/api/v1/auth/login

# ❌ Should return 404
POST https://localhost:5001/api/auth/register
```

### Test 4.2: Courses Endpoints

```http
# ✅ Should work
GET https://localhost:5001/api/v1/courses
POST https://localhost:5001/api/v1/courses

# ❌ Should return 404
GET https://localhost:5001/api/courses
```

### Test 4.3: Lessons Endpoints

```http
# ✅ Should work
GET https://localhost:5001/api/v1/lessons

# ❌ Should return 404
GET https://localhost:5001/api/lessons
```

### ✅ Pass Criteria
- [ ] Tất cả endpoints với `/v1/` hoạt động
- [ ] Endpoints không có `/v1/` return 404

---

## 📊 Test Summary Checklist

### Phase A: Entity Changes
- [x] User.Status enum migration successful
- [x] Course.InstructorId FK created
- [x] Lesson.VideoUrl, OrderIndex added
- [x] Roles seeded (Admin=1, Instructor=2, Student=3)

### Phase B: Authorization Core
- [x] Register assigns Student role automatically
- [x] Login loads user roles with Include/ThenInclude
- [x] JWT contains ClaimTypes.Role claims
- [x] IJwtService signature updated

### Phase C: API Consistency
- [x] All controllers use `/api/v1/` prefix
- [x] Migration applied successfully

### Phase D: Testing (Current)
- [ ] Test Case 1: Register + Role Assignment
- [ ] Test Case 2: Login + JWT Claims
- [ ] Test Case 3: Authorization (403/401/201)
- [ ] Test Case 4: API Versioning

---

## 🐛 Common Issues & Troubleshooting

### Issue 1: "Invalid credentials" khi Login
**Cause:** Password hash mismatch  
**Fix:** Re-register user với đúng password

### Issue 2: JWT không có role claim
**Cause:** User chưa có role trong database  
**Fix:** Check `UserRoles` table, ensure RoleId exists

### Issue 3: 403 Forbidden khi Instructor POST
**Cause:** Role chưa được assign hoặc token expired  
**Fix:** Re-login để get fresh token, verify RoleId = 2

### Issue 4: 500 Internal Server Error
**Cause:** Database connection hoặc missing FK  
**Fix:** Check appsettings.json connection string, verify InstructorId exists

---

## 📝 Test Report Template

```markdown
## Test Execution Report - [Date]

### Environment
- API URL: https://localhost:5001
- Database: PostgreSQL
- Tester: [Your Name]

### Test Results

| Test Case | Status | Notes |
|-----------|--------|-------|
| TC1: Register Flow | ✅ Pass | User created, RoleId=3 assigned |
| TC2: Login JWT Claims | ✅ Pass | Token has role claim |
| TC3.1: Student 403 | ✅ Pass | Correctly forbidden |
| TC3.2: No Token 401 | ✅ Pass | Auth required |
| TC3.3: Instructor 201 | ⚠️ Pending | Need to create instructor user |
| TC4: API Versioning | ✅ Pass | All /v1/ routes work |

### Issues Found
1. [Issue description]
2. [Issue description]

### Recommendations
1. [Recommendation]
```

---

## 🎯 Next Steps After Testing

1. **If all tests pass:**
   - ✅ Mark Tasks 12-14 complete
   - ➡️ Move to Task 15: Create Admin user
   - ➡️ Task 16: Update Swagger documentation

2. **If tests fail:**
   - 🔍 Debug specific endpoint
   - 📋 Document error messages
   - 🐛 Fix code issues
   - 🔄 Re-test

---

**Good luck testing! 🚀**
