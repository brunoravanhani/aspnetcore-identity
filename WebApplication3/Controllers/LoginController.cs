using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private List<User> UserBase = new List<User>
        {
            new User
            {
                UserID = "bruno",
                AccessKey = "123456"
            }
        };

        public object Post([FromBody] User user, [FromServices]SigningConfigurations signingConfig,[FromServices]TokenConfigurations tokenConfig)
        {
            bool credentialsValid = false;
            if (user != null && !String.IsNullOrWhiteSpace(user.UserID))
            {
                var u = UserBase
                    .Where(e => e.UserID == user.UserID)
                    .Where(e => e.AccessKey == user.AccessKey)
                    .FirstOrDefault();
                credentialsValid = (u != null);
            }

            if (credentialsValid)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(user.UserID, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserID)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromSeconds(tokenConfig.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfig.Issuer,
                    Audience = tokenConfig.Audience,
                    SigningCredentials = signingConfig.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });
                var token = handler.WriteToken(securityToken);
                return new
                {
                    authenticated = true,
                    created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK"
                };
            }

            return new
            {
                authenticated = false,
                message = "Falha ao autenticar"
            };
        }
    }
}