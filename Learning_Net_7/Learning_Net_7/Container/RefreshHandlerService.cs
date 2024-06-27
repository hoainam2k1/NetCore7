using AutoMapper;
using Learning_Net_7.Repository;
using Learning_Net_7.Repository.Models;
using Learning_Net_7.Service;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Learning_Net_7.Container
{
    public class RefreshHandlerService : IRefreshHandlerService
    {
        private readonly LearndataContext _context;
        private readonly ILogger<RefreshHandlerService> _logger;
        public RefreshHandlerService(LearndataContext context, ILogger<RefreshHandlerService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<string> GenerateToken(string userName)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenaretor = RandomNumberGenerator.Create())
            {
                randomNumberGenaretor.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
                var existToken = _context.RefreshTokens.FirstOrDefaultAsync(item => item.UserId == userName).Result;
                if (existToken != null)
                {
                    existToken.RefreshTokens = refreshToken;
                }
                else
                {
                    await _context.RefreshTokens.AddAsync(new RefreshToken
                    {
                        UserId = userName,
                        TokenId = new Random().Next().ToString(),
                        RefreshTokens = refreshToken
                    });
                }
                await _context.SaveChangesAsync();
                return refreshToken;
            }
        }
    }
}
