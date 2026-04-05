using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechVault.API.Data;
using TechVault.API.DTOs.Auth;
using TechVault.API.Helpers;
using TechVault.API.Models.Entities;

namespace TechVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly JwtManager _jwtManager;

        public AuthController(AppDbContext context, IMapper mapper, JwtManager jwtManager)
        {
            _context = context;
            _mapper = mapper;
            _jwtManager = jwtManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username))
            {
                return BadRequest(new { message = "Username or Email already exists" });
            }

            var user = _mapper.Map<User>(dto);
            user.Id = Guid.NewGuid();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = "Customer";

            user.CustomerProfile = new CustomerProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber ?? "",
                Address = dto.Address ?? "",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtManager.GenerateToken(user, out var expiresAt);
            var response = _mapper.Map<AuthResponseDto>(user);
            response.ExpiresAt = expiresAt;

            Response.Headers.Append("X-Access-Token", token);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _jwtManager.GenerateToken(user, out var expiresAt);
            var response = _mapper.Map<AuthResponseDto>(user);
            response.ExpiresAt = expiresAt;

            Response.Headers.Append("X-Access-Token", token);

            return Ok(response);
        }
    }
}
