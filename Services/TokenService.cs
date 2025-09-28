using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using User_Management.Model;
using User_Management.ViewModel;

namespace User_Management.Constant
{
    public class TokenService
    {
        public async Task<TokenResponseModel> GenerateTokens(IdentityUser user, IConfiguration config, ApplicationDbContext db)
        {
            var tokenGuid = Guid.NewGuid().ToString();
            var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,tokenGuid)
            };

            var authSiginingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(15),
                claims: claims,
                signingCredentials: new SigningCredentials(authSiginingKey,SecurityAlgorithms.HmacSha256)
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken 
            {
                Token = tokenGuid,
                UserId = user.Id,
                ExpiryDate = DateTime.Now.AddDays(7)
            };

            db.RefreshTokens.Add(refreshToken);
            await db.SaveChangesAsync();

            return new TokenResponseModel 
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Expiration = token.ValidTo
            };
        }
    }
}
