using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShopApp.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserController(ApplicationContext context, IPasswordHasher passwordHasher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string name, string email, string password)
        {
            // Hash the password
            string hashed_password = _passwordHasher.HashPassword(password);

            // Check if the user already exists
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                return NotFound("Такой пользователь уже существует");
            }

            // Create a new user
            User user = new User { Id = Guid.NewGuid(), Name = name, Email = email, Password = hashed_password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user); // Use 'user' instead of 'User' to return the created user
        }
    }



    


    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
