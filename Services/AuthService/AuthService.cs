using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.AuthService
{
  public class AuthService : IAuthService
  {
    private readonly DataContext _context;

    public AuthService(DataContext context)
    {
      _context = context;

    }

    public Task<ServiceResponse<string>> Login(string username, string password)
    {
      throw new NotImplementedException();
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
  }
}
