using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_rpg.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Services.AuthService
{
  public class AuthService : IAuthService
  {
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(DataContext context, IConfiguration configuration)
    {
      _configuration = configuration;
      _context = context;

    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
      var response = new ServiceResponse<string>();
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));

      if (user == null)
      {
        response.Success = false;
        response.Message = "User not found.";
      }
      else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
      {
        response.Success = false;
        response.Message = "Password is incorrect.";
      }
      else
      {
        response.Data = CreateToken(user);
      }

      return response;
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
      ServiceResponse<int> response = new ServiceResponse<int>();

      try
      {
        if (await UserExists(user.Username))
        {
          response.Success = false;
          response.Message = "Username already exists";
        }
        else
        {
          byte[] passwordHash, passwordSalt;
          CreatePasswordHash(password, out passwordHash, out passwordSalt);
          user.PasswordHash = passwordHash;
          user.PasswordSalt = passwordSalt;
          await _context.Users.AddAsync(user);
          await _context.SaveChangesAsync();
          response.Data = user.Id;
        }
      }
      catch (Exception ex)
      {
        response.Success = false;
        response.Message = ex.Message;
      }

      return response;
    }

    public async Task<bool> UserExists(string username)
    {
      if (await _context.Users.AnyAsync(u => u.Username == username))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
      {
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
      }
    }

    private string CreateToken(User user)
    {
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
      };

      var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
        .GetBytes(_configuration.GetSection("AppSettings:Token").Value));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}
