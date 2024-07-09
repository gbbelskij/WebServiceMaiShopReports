using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShopApp.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CreateSaleController: Controller
    {
        private readonly ApplicationContext _context;
        
        public CreateSaleController(ApplicationContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(string name, int amount, int price)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : null;

            if (token == null)
            {
                return Unauthorized("Token is missing.");
            }

            // Расшифровка токена
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Извлечение данных из токена
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token claims.");
            }

            Sale sale = new Sale{SaleId = Guid.NewGuid(), UserId = new Guid(userIdClaim), Name = name, Date = DateTime.UtcNow, Amount = amount, Price = price};
            _context.Sales.Add(sale);
            _context.SaveChanges();
            return Ok("Покупка зарегистрирована.");
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class GetSalesController: Controller
    {
        private readonly ApplicationContext _context;

        public GetSalesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : null;

            if (token == null)
            {
                return Unauthorized("Token is missing.");
            }

            // Расшифровка токена
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Извлечение данных из токена
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token claims.");
            }

            var userSales = _context.Sales.Where(s => s.UserId == new Guid(userIdClaim)).ToList();
            return Ok(userSales);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly ApplicationContext _context;

        public ReportController(ReportService reportService, ApplicationContext context)
        {
            _reportService = reportService;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : null;

            if (token == null)
            {
                return Unauthorized("Token is missing.");
            }

            // Расшифровка токена
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Извлечение данных из токена
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token claims.");
            }

            var userId = new Guid(userIdClaim);

            // Генерация отчета
            var report = _reportService.GenerateUserSalesReport(userId);

            return File(report, "application/pdf", $"UserSalesReport_{userId}.pdf");
        }
    }


}
