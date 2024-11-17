﻿using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Statics
{
    public class ApplicationRoles
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "admin", "member", "doctor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var defaultUser = new AppUser
            {
                UserName = "aminibrahimov52@gmail.com",
                Name = "Amin",
                Surname = "Ibrahimov",
                Email = "aminibrahimov52@gmail.com",
                EmailConfirmed = true
            };

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                var result = await userManager.CreateAsync(defaultUser, "Amin2005");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "admin");
                }
            }
        }
    }
}