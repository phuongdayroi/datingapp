using DatingApp.API.Data.IRepository;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace DatingApp.API.Data.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatingAppDbContext _context;
        public AuthRepository(DatingAppDbContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                return null;
            }
            if (!SecurityHelpers.Authentication.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;
            return user;
        }
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            SecurityHelpers.Authentication.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();


            return user;
        }
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
            {
                return true;
            }
            return false;
        }
    }

}
