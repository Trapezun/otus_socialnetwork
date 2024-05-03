using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialNetwork.Classes.Services
{
    public class TokenJWTService
    {
        public TokenJWTService(string issuer, string audience,
         string encryptingKey,
         string signingKey)
        {
            _issuer = issuer;
            _audience = audience;
            _encryptingKey = encryptingKey;
            _signingKey = signingKey;
        }
        private string _issuer;
        private string _audience;
        private string _encryptingKey;
        private string _signingKey;


        public string CreateToken(string id, TimeSpan timeout)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptingKey = Encoding.ASCII.GetBytes(_encryptingKey);
            var signingKey = Encoding.ASCII.GetBytes(_signingKey);

            var now = DateTime.UtcNow;

            var subject = generateClaimsIdentity(id);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Subject = subject,
                Expires = now.Add(timeout),
                EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptingKey), JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedJwt;

        }


        public string GetID(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.User != null)
            {
                var cl = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid);
                if (cl != null) {
                    return cl.Value;
                }             
            }
            return "";
        }

        private ClaimsIdentity generateClaimsIdentity(string id)
        {
            var claimList = new List<Claim> {
                new Claim(ClaimTypes.Sid, id)
            };
            var identity = new ClaimsIdentity(claimList, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return identity;
        }


    }
}
