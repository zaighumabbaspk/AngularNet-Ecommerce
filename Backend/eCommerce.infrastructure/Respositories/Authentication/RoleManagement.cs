
using eCommerce.Domain.Entities.Identity;
using eCommerce.infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCommerce.Infrastructure.Data;

using System.Threading.Tasks;
using eCommerce.Domain.Services.Interfaces.Authentication;



namespace eCommerce.infrastructure.Respositories.Authentication
{
    public class RoleManagement(UserManager<AppUser> UserManager) : IRoleManagement
    {
        public async Task<bool> AddUserToRole(AppUser user, string roleName) => 
         (await UserManager.AddToRoleAsync(user, roleName)).Succeeded;




        public async Task<string?> GetUserRole(string userEmail)
        {
            var user = await UserManager.FindByEmailAsync(userEmail);

            if (user == null) return null;

            return (await UserManager.GetRolesAsync(user!)).FirstOrDefault();
        }













    }


}

