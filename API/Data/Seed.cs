using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using API.Entities;
using System.Security.Cryptography;
using System.Text;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            // If user have any data then return.
            if(await context.Users.AnyAsync()) return;

            // If user doesn't have any data then add json data.
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            System.Console.WriteLine(userData);
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                // using common password for all seed users.
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Jairaj!@#"));
                user.PasswordSalt = hmac.Key;
                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}