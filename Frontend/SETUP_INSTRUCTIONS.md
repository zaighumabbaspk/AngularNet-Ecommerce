# Authentication Setup - Final Steps

## ✅ What Has Been Completed:

1. ✅ Auth Models created
2. ✅ Auth Service with JWT management
3. ✅ Auth Guards (authGuard, adminGuard)
4. ✅ HTTP Interceptor for automatic token injection
5. ✅ Login Component (complete)
6. ✅ Signup Component (complete)
7. ✅ app.config.ts updated with interceptor
8. ✅ app.routes.ts updated with auth routes
9. ✅ Header component updated with login/logout buttons
10. ✅ Product component updated with admin-only features

---

## 📦 Required Package Installation:

Run this command in the Frontend directory:

```bash
npm install jwt-decode
```

---

## 🚀 How to Test:

### 1. Start the Backend
```bash
cd Backend/eCommerce.Host
dotnet run
```

### 2. Start the Frontend
```bash
cd Frontend
npm install jwt-decode
npm start
```

### 3. Test Authentication Flow

**A. Create Admin User (Manual Database Setup):**

First, create a regular user account:
1. Go to `http://localhost:4200/signup`
2. Fill in the form:
   - Full Name: Admin User
   - Email: admin@example.com
   - Password: Admin@123
   - Confirm Password: Admin@123
3. Click "Create Account"

Then, assign Admin role in the database:
1. Open your database management tool (SQL Server Management Studio, Azure Data Studio, etc.)
2. Run these SQL queries:

```sql
-- Find the user ID
SELECT Id, Email FROM AspNetUsers WHERE Email = 'admin@example.com';

-- Find the Admin role ID
SELECT Id, Name FROM AspNetRoles WHERE Name = 'Admin';

-- Assign Admin role (replace the IDs with actual values from above queries)
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES 
  ('your-user-id-here', 'admin-role-id-here');
```

**B. Login as Admin:**
1. Go to `http://localhost:4200/login`
2. Login with:
   - Email: admin@example.com
   - Password: Admin@123
3. You should see your name in the header
4. You should see "Add New Product" button (admin only)
5. You should see Edit/Delete buttons on products (admin only)

**C. Create Regular User:**
1. Logout
2. Go to `/signup` again
3. Create another account
4. Login with the new account
5. You should NOT see admin features (this is the default behavior)

**D. Test Protected Features:**
1. Try to add a product (only admin can)
2. Try to edit a product (only admin can)
3. Try to delete a product (only admin can)

---

## 🔐 Authentication Features:

### Token Management:
- JWT tokens stored in localStorage
- Automatic token refresh on 401 errors
- Tokens sent with every API request

### Role-Based Access:
- All users created via signup = User role
- Admin role must be assigned manually in database
- Admin can: Add, Edit, Delete products
- Users can: View products, add to cart

### Security:
- Password validation (min 6 characters)
- Email validation
- Password confirmation matching
- Protected routes with guards
- Automatic logout on token expiration

---

## 🎨 UI Features:

### Login/Signup Pages:
- Beautiful gradient background
- Glassmorphism cards
- Form validation with error messages
- Password visibility toggle
- Loading states
- Success/Error alerts
- Smooth animations

### Header:
- Shows user name when logged in
- Login/Signup buttons when logged out
- Logout button with icon
- Responsive design

### Product Page:
- Admin features hidden for regular users
- Add Product button (admin only)
- Edit/Delete buttons (admin only)
- All users can view and search products

---

## 🔧 Backend Authorization (Optional):

If you want to enforce authorization on the backend, add these attributes to your controllers:

```csharp
// In ProductController.cs

[HttpPost("add")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Add(CreateProduct product)
{
    // Only admins can add products
}

[HttpPut("update")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Update(UpdateProduct product)
{
    // Only admins can update products
}

[HttpDelete("delete/{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Delete(Guid id)
{
    // Only admins can delete products
}

[HttpGet("all")]
[AllowAnonymous]
public async Task<IActionResult> GetAll()
{
    // Everyone can view products
}
```

---

## 📱 Testing Checklist:

- [ ] Install jwt-decode package
- [ ] Backend is running
- [ ] Frontend is running
- [ ] Can create account
- [ ] Can login
- [ ] Can see user name in header
- [ ] Can logout
- [ ] First user is admin
- [ ] Admin can see "Add Product" button
- [ ] Admin can see Edit/Delete buttons
- [ ] Regular users cannot see admin features
- [ ] Token is stored in localStorage
- [ ] Token is sent with API requests
- [ ] Auto-redirect to login when not authenticated

---

## 🐛 Troubleshooting:

### Issue: "Cannot find module 'jwt-decode'"
**Solution:** Run `npm install jwt-decode`

### Issue: Login not working
**Solution:** 
1. Check backend is running
2. Check API URL in environment.ts
3. Check browser console for errors
4. Check network tab for API calls

### Issue: Admin features not showing
**Solution:**
1. Make sure you assigned Admin role in the database
2. Check browser console for role in token
3. Clear localStorage and login again
4. Verify the role assignment with this SQL:
```sql
SELECT u.Email, r.Name as Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'your-email@example.com';
```

### Issue: Token not being sent
**Solution:**
1. Check app.config.ts has authInterceptor
2. Check browser DevTools → Network → Headers
3. Should see "Authorization: Bearer <token>"

---

## 🎯 Next Steps:

1. Install jwt-decode: `npm install jwt-decode`
2. Test the authentication flow
3. Create admin and user accounts
4. Test role-based features
5. Implement cart functionality (next feature)

---

## 📚 File Structure:

```
Frontend/src/app/
├── Core/
│   ├── Guards/
│   │   └── auth.guard.ts
│   ├── Interceptors/
│   │   └── auth.interceptor.ts
│   ├── Models/
│   │   └── auth.model.ts
│   └── Services/
│       └── auth.service.ts
├── Shared/
│   └── Componets/
│       ├── auth/
│       │   ├── login/
│       │   │   ├── login.component.ts
│       │   │   ├── login.component.html
│       │   │   └── login.component.css
│       │   └── signup/
│       │       ├── signup.component.ts
│       │       ├── signup.component.html
│       │       └── signup.component.css
│       ├── header/
│       │   └── (updated with auth buttons)
│       └── product/
│           └── (updated with admin features)
├── app.config.ts (updated)
└── app.routes.ts (updated)
```

---

## ✨ Congratulations!

Your authentication system is now complete with:
- ✅ JWT Authentication
- ✅ Refresh Tokens
- ✅ Role-Based Access Control
- ✅ Protected Routes
- ✅ Beautiful UI
- ✅ Form Validation
- ✅ Error Handling
- ✅ Automatic Token Management

Just install jwt-decode and start testing!
