using ecommerce.Interfaces;
using ecommerce.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using System.Linq;
using ecommerce.Models.DbModel;
using ecommerce.Models.Dto;

namespace BookManagementSystem.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly byte[] _secret;

        public TokenService(IConfiguration config)
        {
            // Initializing the symmetric security key using the token key from the configuration.
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _secret = Encoding.UTF8.GetBytes(config["TokenKey"]);
        }

        public string CreateToken(UserModel user)
        {
            // Creating a list of claims for the token.
            var claims = new List<Claim>
            {
                // Adding a claim representing the NameId with the value of the user's UserName.
                new Claim("Id", user.Id.ToString())
            };

            // Creating signing credentials using the symmetric security key.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Creating a token descriptor to define the properties of the JWT token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Setting the subject (claims identity) of the token.
                Subject = new ClaimsIdentity(claims),

                // Setting the expiration date of the token (7 days from the current date).
                Expires = DateTime.Now.AddDays(30),

                // Setting the signing credentials for the token.
                SigningCredentials = creds,
            };

            // Creating an instance of JwtSecurityTokenHandler to generate and write JWT tokens.
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generating the JWT token using the token descriptor.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Returning the string representation of the JWT token.
            return tokenHandler.WriteToken(token);
        }

        public int ValidateToken(string token)
        {
            if (token == null)
                return -1;

            var handler = new JwtSecurityTokenHandler();
            var key = _secret;
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken ValidatedToken);

                var jwtToken = (JwtSecurityToken)ValidatedToken;
                int UserId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);
                return UserId;

            }
            catch
            {
                return -1;
            }
        }

        public string ValidateVerifyToken(string token)
        {
            if (token == null)
                return null;

            var handler = new JwtSecurityTokenHandler();
            var key = _secret;
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken ValidatedToken);

                var jwtToken = (JwtSecurityToken)ValidatedToken;
                string UserEmail = jwtToken.Claims.First(x => x.Type == "Email").Value;
                return UserEmail;

            }
            catch
            {
                return null;
            }
        }
        public string VerificationGenerateEmail(SignupDto user)
        {
            var claims = new List<Claim>
            {
                // Adding a claim representing the NameId with the value of the user's UserName.
                new Claim("Email", user.Email)
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Setting the subject (claims identity) of the token.
                Subject = new ClaimsIdentity(claims),

                // Setting the expiration date of the token (7 days from the current date).
                Expires = DateTime.Now.AddDays(30),

                // Setting the signing credentials for the token.
                SigningCredentials = creds,
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generating the JWT token using the token descriptor.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Returning the string representation of the JWT token.
            return tokenHandler.WriteToken(token);
        }
    }
}
