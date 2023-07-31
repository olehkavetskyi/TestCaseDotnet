using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestCaseDotnet.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public TokenService() { }

    public TokenService(IConfiguration config, SymmetricSecurityKey key)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]!));
    }

    public string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: _config["Token:Issuer"],
            audience: _config["token:Audience"],
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
