using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TokenService(IOptions<JwtSettings> options, IDateTimeProvider dateTimeProvider)
    {
        _settings = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public LoginResponseDto CreateToken(AuthenticatedUser user)
    {

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimNames.Name, user.Username),
            new Claim(ClaimNames.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = _dateTimeProvider.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponseDto(accessToken, expires, user.Username, user.Role);
    }
}
