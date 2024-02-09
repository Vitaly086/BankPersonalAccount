using System.IdentityModel.Tokens.Jwt;

namespace BankAccountGateway.Extensions;

public static class HttpContextExtensions
{
    public static JwtSecurityToken? ReadJwtToken(this HttpContext httpContext, string tokenName = "Authorization")
    {
        var authorizationHeader = httpContext.Request.Headers[tokenName].FirstOrDefault();
        if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
        {
            return null;
        }

        var tokenStr = authorizationHeader.Substring("Bearer ".Length).Trim();

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ReadJwtToken(tokenStr);
    }
}