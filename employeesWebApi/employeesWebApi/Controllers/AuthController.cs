using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace employeesWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("token")]   //So to access: api/auth/token to access this endpoint of our api
        public ActionResult GetToken()
        {

            // 1. Need security key, which we use to sign the token, s.t. we can validate it later
            // Just a string. Can save it anywhere in DB, config file, or envt. 
            // Let's put in envt s.t. we won't accidentally commit it to code version server, and it 
            // will always be secured. But later, put this key somewhere safe.
            string securityKey = "this_is_the_very_long_security_key_for_token_validation_keep_this_a_secret_somehow";

            // 2. Need symmetric security key
            // we pass our securityKey to it.
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));


            // 3. Need to create credentials for signing the token
            // Pass our symmetricSecurityKey. Then choose algo for generating, sign, and validate the token 
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);


            // 3a. Add claims (user type in JWT payload)
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));  // Don't forget to assign this claim to the token body, in #4.
            claims.Add(new Claim(ClaimTypes.Role, "User"));
            claims.Add(new Claim("Custom_Claim", "Custom_value"));


            // 4. Need to create the token
            var token = new JwtSecurityToken(
                issuer: "theIssuer", //any string
                audience: "readers",
                expires: DateTime.Now.AddHours(1), // expires in 1 hr
                signingCredentials: signingCredentials,
                claims: claims
                );

            // 5. Return token
            // We need the JWT SecurityTokenHandler to return, from this token, a string that can be used in request to our API
            // The WriteToken will return string version of token, which we return to the client
            return Ok(new JwtSecurityTokenHandler().WriteToken(token)); 
        }

    }
}