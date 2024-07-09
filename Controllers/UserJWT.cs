using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShopApp.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class UserJWT : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserJWT(ApplicationContext context, IPasswordHasher passwordHasher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string email, string password)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (existingUser == null)
            {
                return BadRequest("Такого пользователя не существует.");
            }

            bool IsPasswordValid = _passwordHasher.VerifyPassword(password, existingUser.Password);

            if (!IsPasswordValid)
            {
                return BadRequest("Неверный пароль.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(JwtRegisteredClaimNames.Sub, existingUser.Id.ToString()), // Приведение Id к строке
                new Claim(JwtRegisteredClaimNames.Email, existingUser.Email)
            };

            var key = AuthOptions.GetSymmetricSecurityKey();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
