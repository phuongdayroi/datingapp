using DatingApp.API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DatingApp.API.Data.Seeds
{
    public class Seed
    {
        public static void SeedUsers(DatingAppDbContext context)
        {
            if (!context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var item in users)
                {
                    byte[] passwordSalt, passwordHash;
                    SecurityHelpers.Authentication.CreatePasswordHash("password", out passwordHash, out passwordSalt);
                    item.PasswordHash = passwordHash;
                    item.PasswordSalt = passwordSalt;
                    item.Username = item.Username.ToLower();
                    context.Users.Add(item);
                }
                context.SaveChanges();
            }
        }
    }
}
