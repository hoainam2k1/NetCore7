using Learning_Net_7.Modal;
using Learning_Net_7.Repository;
using Learning_Net_7.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Learning_Net_7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearndataContext _service;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshHandlerService _refreshHandlerService;
        public AuthorizeController(LearndataContext service, IOptions<JwtSettings> options, IRefreshHandlerService refreshHandlerService)
        {
            _service = service;
            _jwtSettings = options.Value;
            _refreshHandlerService = refreshHandlerService;
        }
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCredential userCredential)
        {
            var user = _service.Users.FirstOrDefaultAsync(item => item.UserName == userCredential.UserName && item.Password == userCredential.Password).Result;
            if(user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey ?? "");
                var tokenDesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.Now.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenHandler.CreateToken(tokenDesc);
                var finalToken = tokenHandler.WriteToken(token);
                return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await _refreshHandlerService .GenerateToken(userCredential.UserName ?? "")});
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] TokenResponse token)
        {
            var _refreshToken = _service.RefreshTokens.FirstOrDefaultAsync(item => item.RefreshTokens == token.RefreshToken).Result;
            if(_refreshToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey ?? "");

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;
                if(_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string userName = principal.Identity?.Name ?? "";
                    var _existData = _service.RefreshTokens.FirstOrDefaultAsync(item => item.UserId == userName && item.RefreshTokens == token.RefreshToken);
                    if(_existData != null)
                    {
                        var _newToken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey ?? ""))
                            , SecurityAlgorithms.HmacSha256));

                        var _finaltoken = tokenHandler.WriteToken(_newToken);
                        return Ok(new TokenResponse() { Token = _finaltoken, RefreshToken = await _refreshHandlerService.GenerateToken(userName)});

                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized() ;
                }
                //var tokenDesc = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //        new Claim(ClaimTypes.Name, user.UserName),
                //        new Claim(ClaimTypes.Role, user.Role)
                //    }),
                //    Expires = DateTime.Now.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                //};
                //var token = tokenHandler.CreateToken(tokenDesc);
                //var finalToken = tokenHandler.WriteToken(token);
                //return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await _refreshHandlerService .GenerateToken(userCredential.UserName ?? "")});
            }
            else
            {
                return Unauthorized();
            }
            
        }
    }
}
