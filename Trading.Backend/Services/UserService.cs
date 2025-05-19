using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Trading.Backend.Models;
using Trading.Backend.Persistance;

namespace Trading.Backend.Services
{
    public class UserService
    {
 
        private readonly IServiceScopeFactory _scopeFactory;
        public UserService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public User CreateUser(string userId)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var user = db.Users.Where(u => u.UserId == userId).Include(u => u.TradeRecords).SingleOrDefault();

            if (user == null)
            {
                user = new User
                {
                    UserId = userId,
                    Name = userId
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
            return user;
        }
        
    }
}
