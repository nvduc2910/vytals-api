
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Vytals.Helpers
{
    public class Token
    {
        public static string DecodingToken(string token)
        {
            if (token != null && token != string.Empty)
            {
                token = token.Split(' ').LastOrDefault();
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var claimId = tokenS.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                return claimId;
            }
            return string.Empty;
        }
    }
}
