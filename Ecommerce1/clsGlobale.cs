using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce1
{
    public static class clsGlobale
    {
        private static readonly IConfigurationRoot _configuration;

        // Static constructor to initialize the configuration
        static clsGlobale()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        // Method to retrieve values
        public static string GetJwtSecret()
        {
            return _configuration["AppSettings:JWT_SECRET"];
        }

        public static string SetImageURL(string ImageName)
        {
            return _configuration["ImagesUrl"].ToString()+ImageName;
        }

        public static string GetEmail()
        {
            return _configuration["Email"];
        }

        public static string GetVerifyEmailLink()
        {
            return _configuration["GetVerifyEmailLink"];

        }
        public static string GetEmailPassWord()
        {
            return _configuration["EmailPassWord"];
        }

        public static string GetLoadDiractory()
        {
            return _configuration["UploadDiractory"];
        }
        public static string? ConectionString()
        {
            return _configuration["ConnectionString"];
        }

        public static string? GenerateJwtToken(DTOUser user)
        {
            if (string.IsNullOrEmpty(GetJwtSecret())) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // **New method to extract UserID from a token**
        public static int? ExtractUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(GetJwtSecret()))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
            }
            catch
            {
                return null; // Invalid token
            }
        }
    }
}
