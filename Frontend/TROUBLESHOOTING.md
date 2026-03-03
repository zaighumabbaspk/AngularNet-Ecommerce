# Troubleshooting Guide

## ✅ Package Installed
- jwt-decode has been installed successfully

## Common Errors and Solutions:

### 1. **Error: Cannot find module 'jwt-decode'**
**Status:** ✅ FIXED
**Solution:** Package has been installed via `npm install jwt-decode`

---

### 2. **Error: Property 'success' does not exist on type 'ServiceResponse'**
**Possible Cause:** Backend ServiceResponse might have different property names

**Solution:** Check if backend returns `Success` (capital S) instead of `success` (lowercase s)

If backend uses capital S, update `Frontend/src/app/Core/Models/auth.model.ts`:
```typescript
export interface LoginResponse {
  success: boolean;  // Change to: Success: boolean; if backend uses capital
  message: string;   // Change to: Message: string; if backend uses capital
  token: string | null;
  refreshToken: string | null;
  email: string | null;
  userId: string | null;
}
```

---

### 3. **Error: CORS Policy Error**
**Possible Cause:** Backend CORS not configured properly

**Solution:** Your backend already has CORS configured in `Program.cs`, but verify it's working:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
```

---

### 4. **Error: 401 Unauthorized on API calls**
**Possible Cause:** Token not being sent or backend authentication not configured

**Solution:** 
1. Check browser DevTools → Network → Headers
2. Should see: `Authorization: Bearer <token>`
3. If not, check that `authInterceptor` is registered in `app.config.ts`

---

### 5. **Error: Cannot read properties of null (reading 'role')**
**Possible Cause:** JWT token claims have different property names

**Solution:** Check your JWT token structure. Open browser console and run:
```javascript
localStorage.getItem('token')
```

Then decode it at https://jwt.io to see the claim names.

Update `auth.service.ts` `getUserFromToken()` method with correct claim names:
```typescript
private getUserFromToken(token: string): User | null {
  try {
    const decoded: any = jwtDecode(token);
    console.log('Decoded token:', decoded); // Debug: see actual claims
    return {
      userId: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || '',
      email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '',
      fullName: decoded['FullName'] || '',
      role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'User'
    };
  } catch (error) {
    console.error('Error decoding token:', error);
    return null;
  }
}
```

---

### 6. **Error: Backend returns 404 on /api/Authentication/create**
**Possible Cause:** Backend endpoint might be different

**Solution:** Check your backend `AuthenticationController.cs`:
- Endpoint should be: `[HttpPost("create")]`
- Full URL: `https://localhost:7137/api/Authentication/create`

If different, update `auth.service.ts`:
```typescript
signup(request: SignupRequest): Observable<any> {
  return this.http.post(`${this.apiUrl}/create`, request);
  // Change 'create' to match your backend endpoint
}
```

---

### 7. **Error: Signup form validation not working**
**Possible Cause:** Backend expects different field names

**Solution:** Check backend `CreateUser` DTO. It should match:
```csharp
public class CreateUser : BaseModel
{
    public required string Fullname { get; set; }  // Note: capital F
    public required string ConfirmPassword { get; set; }
}
```

If backend uses `FullName` (capital N), update signup component:
```typescript
this.signupForm = this.fb.group({
  fullname: ['', [Validators.required, Validators.minLength(3)]],
  // Change to: FullName if backend expects capital letters
});
```

---

### 8. **Error: Admin features not showing**
**Possible Cause:** Role claim not being read correctly

**Solution:** 
1. Login as first user
2. Open browser console
3. Run: `localStorage.getItem('token')`
4. Decode at jwt.io
5. Check if role claim exists and matches 'Admin'

If role claim name is different, update `getUserFromToken()` in auth.service.ts

---

### 9. **Error: Refresh token not working**
**Possible Cause:** Backend endpoint might be different

**Solution:** Check backend endpoint format:
```csharp
[HttpGet("refreshToken/{refreshToken}")]
public async Task<IActionResult> ReviveToken(string refreshToken)
```

Should match frontend:
```typescript
refreshToken(): Observable<LoginResponse> {
  const refreshToken = this.authState.value.refreshToken;
  return this.http.get<LoginResponse>(`${this.apiUrl}/refreshToken/${refreshToken}`);
}
```

---

### 10. **Error: Module not found errors**
**Solution:** Run these commands:
```bash
cd Frontend
npm install
npm install jwt-decode
```

---

## 🔍 Debugging Steps:

### Step 1: Check Browser Console
Open DevTools (F12) → Console tab
Look for any red error messages

### Step 2: Check Network Tab
Open DevTools (F12) → Network tab
1. Try to login
2. Look for the login API call
3. Check:
   - Request URL
   - Request Headers
   - Request Body
   - Response Status
   - Response Body

### Step 3: Check LocalStorage
Open DevTools (F12) → Application tab → LocalStorage
Should see:
- `token`: JWT token string
- `refreshToken`: Refresh token string

### Step 4: Verify Backend is Running
1. Backend should be running on `https://localhost:7137`
2. Try accessing: `https://localhost:7137/api/Authentication/login` in browser
3. Should see Swagger UI or API response

### Step 5: Check Environment Configuration
File: `Frontend/src/app/environment/environment.ts`
```typescript
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7137/api'  // Verify this matches your backend
};
```

---

## 🧪 Quick Test Commands:

### Test Backend API (PowerShell):
```powershell
# Test if backend is running
Invoke-WebRequest -Uri "https://localhost:7137/api/Product/all" -SkipCertificateCheck

# Test signup
$body = @{
    fullname = "Test User"
    email = "test@example.com"
    password = "Test@123"
    confirmPassword = "Test@123"
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://localhost:7137/api/Authentication/create" `
    -Method POST `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck
```

---

## 📋 Checklist:

- [x] jwt-decode installed
- [ ] Backend is running
- [ ] Frontend is running
- [ ] No console errors
- [ ] Can access login page
- [ ] Can access signup page
- [ ] Backend API responds
- [ ] CORS is configured
- [ ] Environment URL is correct

---

## 🆘 Still Having Issues?

1. **Clear browser cache and localStorage:**
   - Open DevTools (F12)
   - Application tab → LocalStorage → Clear All
   - Hard refresh: Ctrl+Shift+R

2. **Restart both servers:**
   ```bash
   # Stop both servers (Ctrl+C)
   # Start backend
   cd Backend/eCommerce.Host
   dotnet run
   
   # Start frontend (new terminal)
   cd Frontend
   npm start
   ```

3. **Check specific error message:**
   - Copy the exact error from console
   - Check which file/line it's coming from
   - Look for that section in the code

---

## 📞 Common Error Messages:

| Error | Likely Cause | Solution |
|-------|-------------|----------|
| "Cannot find module 'jwt-decode'" | Package not installed | ✅ Already fixed |
| "CORS policy error" | Backend CORS issue | Check Program.cs CORS config |
| "401 Unauthorized" | Token not sent/invalid | Check interceptor, check token in localStorage |
| "404 Not Found" | Wrong API endpoint | Check environment.ts and backend routes |
| "Cannot read property 'role'" | Token decode issue | Check getUserFromToken() method |
| "Property 'success' does not exist" | DTO mismatch | Check backend response structure |

---

## ✅ Everything Working?

If authentication is working:
1. You should see login/signup pages
2. You can create an account
3. You can login
4. You see your name in header
5. First user sees admin features
6. Token is in localStorage
7. API calls include Authorization header

Next step: Test the cart functionality!
