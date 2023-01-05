using CloneMessengerBackendAPI.Service.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.BaseModels
{
    public static class Authentication
    {
        public static string GenerateJWT(UserModel user)
        {
            //var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.NameIdentifier,user.Username),
            //    new Claim(JwtRegisteredClaimNames.Jti,user.Id.ToString()),
            //};
            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            //var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            //var expireDays = DateTime.Now.AddDays(Convert.ToDouble(ConfigurationManager.AppSettings["config:JwtExpireDays"]));

            //var token = new JwtSecurityToken(
            //    Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
            //    Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
            //    claims,
            //    null,
            //    expires:expireDays,
            //    signingCredentials:credentials
            //    );
            //return new JwtSecurityTokenHandler().WriteToken(token);


            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.GivenName,user.DisplayName),
                new Claim(ClaimTypes.UserData,user.Password),
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            return GetJwt(claimsIdentity);
        }
        public static string GetJwt(ClaimsIdentity claimsIdentity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expireDays = DateTime.Now.AddDays(Convert.ToDouble(ConfigurationManager.AppSettings["config:JwtExpireDays"]));

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = jwtSecurityTokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Issuer = Convert.ToString(ConfigurationManager.AppSettings["config:JwtIssuer"]),
                Audience = Convert.ToString(ConfigurationManager.AppSettings["config:JwtAudience"]),
                SigningCredentials = credentials,
                Expires = expireDays
            });
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static ClaimsPrincipal ParseToken(string jwt)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            if (!((jwtSecurityTokenHandler).ReadToken(jwt) is JwtSecurityToken))
            {
                return null;
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateAudience = true,
                ValidAudience = ConfigurationManager.AppSettings["config:JwtAudience"],
                ValidateIssuer = true,
                ValidIssuer = ConfigurationManager.AppSettings["config:JwtIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
            };
            SecurityToken validatedToken;
            return jwtSecurityTokenHandler.ValidateToken(jwt, validationParameters, out validatedToken);
        }
    }
}
