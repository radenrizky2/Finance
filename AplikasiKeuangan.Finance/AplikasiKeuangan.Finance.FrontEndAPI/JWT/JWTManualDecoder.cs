using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AplikasiKeuangan.Finance.FrontEndAPI.JWT
{
    public class JWTManualDecoder
    {
        private readonly ILogger<JWTManualDecoder> _logger;

        public JWTManualDecoder(ILogger<JWTManualDecoder> log)
        {
            _logger = log;
        }

        [FunctionName("JWTManualDecoder")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var audience = Environment.GetEnvironmentVariable("Nexus.Identity.Audience");
            var issuer = Environment.GetEnvironmentVariable("Nexus.Identity.Issuer");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var jwtDTO = JsonConvert.DeserializeObject<JWTDTO>(requestBody);
            var result = await ValidateByReturnClaims(audience, issuer, jwtDTO.PublicKey, jwtDTO.JWT);

            return new OkObjectResult(result);
        }

        private static async Task<UserClaimsDTO> ValidateByReturnClaims(string audience, string issuer, string publicKey, string jwt)
        {
            string publicKeyRaw = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(publicKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = audience,
                ValidIssuer = issuer,
                ValidateLifetime = true,
                IssuerSigningKey = new RsaSecurityKey(JsonConvert.DeserializeObject<RSAParameters>(publicKeyRaw))
            };

            var claimsPrincipal = tokenHandler.ValidateToken(jwt, validationParameters, out var validatedToken);

            var result = new UserClaimsDTO()
            {
                Name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value,
                NameIdentifier = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Roles = claimsPrincipal.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList()
            };

            return result;
        }

    }
}


#region Pseudocode ValidateByReturnClaims

//function ValidateByReturnClaims(audience: string, issuer: string, publicKey: string, jwt: string) -> UserClaimsDTO:
//publicKeyRaw = decodeBase64(publicKey) // decode base64-encoded public key

//    tokenHandler = new JwtSecurityTokenHandler()
//    validationParameters = new TokenValidationParameters
//    {
//        ValidAudience = audience,
//        ValidIssuer = issuer,
//        ValidateLifetime = true,
//        IssuerSigningKey = new RsaSecurityKey(deserializeRsaParams(publicKeyRaw))
//    }

//    claimsPrincipal = tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken) // validate JWT token

//    result = new UserClaimsDTO()
//    result.Name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value // extract "Name" claim from claimsPrincipal, if it exists
//    result.NameIdentifier = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value // extract "NameIdentifier" claim from claimsPrincipal, if it exists
//    result.Roles = claimsPrincipal.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList() // extract "Role" claims from claimsPrincipal, if they exist

//    return result


#endregion

#region Pseudocode with Explanation

//function ValidateByReturnClaims(audience: string, issuer: string, publicKey: string, jwt: string) -> UserClaimsDTO:
//This declares a function called ValidateByReturnClaims that takes four arguments: audience, issuer, publicKey, and jwt.The function returns an object of type UserClaimsDTO.

//publicKeyRaw = decodeBase64(publicKey)
//This line decodes the publicKey argument from its base64-encoded format into a raw byte array using the decodeBase64 function.The resulting byte array is stored in the publicKeyRaw variable.

//tokenHandler = new JwtSecurityTokenHandler()
//This creates a new instance of JwtSecurityTokenHandler, which is a class that can be used to validate and manipulate JSON Web Tokens (JWTs).

//validationParameters = new TokenValidationParameters
//                       {
//                           ValidAudience = audience,
//                           ValidIssuer = issuer,
//                           ValidateLifetime = true,
//                           IssuerSigningKey = new RsaSecurityKey(deserializeRsaParams(publicKeyRaw))
//                       }
//This creates a new instance of TokenValidationParameters, which contains the parameters that will be used to validate the JWT. The ValidAudience and ValidIssuer properties are set to the audience and issuer arguments, respectively, to ensure that the token was issued for the correct audience and by the correct issuer. The ValidateLifetime property is set to true to ensure that the token has not expired. Finally, the IssuerSigningKey property is set to a new instance of RsaSecurityKey that is initialized with the public key data from publicKeyRaw.

//claimsPrincipal = tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken)
//This line uses the ValidateToken method of the JwtSecurityTokenHandler class to validate the JWT specified by the jwt argument. The validationParameters object specifies the parameters that will be used to validate the token, and the validatedToken variable is used to store information about the validated token (such as its expiration time).

//result = new UserClaimsDTO()
//This creates a new instance of UserClaimsDTO, which is the object that will be returned by the function.

//result.Name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value
//result.NameIdentifier = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
//result.Roles = claimsPrincipal.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList()
//These lines extract information from the validated JWT and populate the corresponding properties of the result object. Specifically, the Name property is set to the value of the "Name" claim in the JWT (if it exists), the NameIdentifier property is set to the value of the "NameIdentifier" claim (if it exists), and the Roles property is set to a list of the values of all "Role" claims (if any exist) in the JWT.

//return result
//This line returns the result object from the function, which contains the extracted information from the validated JWT.

#endregion

#region Step By Step

//The function takes four input parameters: audience, issuer, publicKey, and jwt.
//The publicKey parameter is Base64-encoded, so the first thing the function does is decode it to a raw string using Convert.FromBase64String().
//The function creates a new instance of JwtSecurityTokenHandler.
//The function creates a new instance of TokenValidationParameters and sets its properties to the values passed in as parameters. Specifically, it sets ValidAudience to the audience parameter, ValidIssuer to the issuer parameter, ValidateLifetime to true, and IssuerSigningKey to an RsaSecurityKey created from the publicKeyRaw string.
//The function calls ValidateToken() on the tokenHandler instance, passing in the jwt parameter, the validationParameters object, and an out parameter called validatedToken. This method validates the token against the provided parameters, and if successful, returns a ClaimsPrincipal object that represents the authenticated entity.
//The function creates a new instance of UserClaimsDTO.
//The function populates the Name, NameIdentifier, and Roles properties of the result object using data from the claimsPrincipal object. Specifically, it uses the FindFirst() method to find the first claim of type ClaimTypes.Name and get its value (if it exists), and does the same for ClaimTypes.NameIdentifier and ClaimTypes.Role. If there are multiple ClaimTypes.Role claims, it uses LINQ to create a new list of their values.
//The function returns the result object.

#endregion


#region Node Js Version - UNTESTED

//const jwt = require('jsonwebtoken');

//async function validateByReturnClaims(audience, issuer, publicKey, jwt)
//{
//    const publicKeyRaw = Buffer.from(publicKey, 'base64').toString('ascii');

//    const decodedToken = jwt.decode(jwt, { complete: true });
//const publicKey = JSON.parse(publicKeyRaw);
//const verificationOptions = {
//    audience: audience,
//    issuer: issuer,
//    algorithms: ['RS256'],
//    ignoreExpiration: false,
//    key: publicKey
//  };

//const verifiedToken = jwt.verify(jwt, verificationOptions);

//const result = {
//    Name: verifiedToken.Name,
//    NameIdentifier: verifiedToken.NameIdentifier,
//    Roles: verifiedToken.Roles
//  };

//return result;
//}

#endregion