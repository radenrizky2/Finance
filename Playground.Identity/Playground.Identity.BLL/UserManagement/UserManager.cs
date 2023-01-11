using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Nexus.Base.CosmosDBRepository;
using Nexus.Base.Identity;
using System.Security.Claims;
using static Playground.Identity.DAL.Repositories;

namespace Playground.Identity.BLL.UserManagement
{
    public class UserManager
    {
       
        public static async Task<DAL.Model.User> CreateUser(
             UserRepository repsUser, DAL.Model.User user,
            ILogger log, string createdBy = null
            )
        {
            return await repsUser.CreateAsync(user, null, createdBy, null);
        }


        public static async Task<DAL.Model.User> GetUserById(
            UserRepository repsUser, string userId, ILogger log
            )
        {
            var result = (await repsUser.GetAsync(
                predicate: p => p.Id == userId)).Items.FirstOrDefault();

            return result;
        }

        public static async Task<PageResult<DAL.Model.User>> GetAllUser(
           UserRepository repsUser, string continuationToken,
           ILogger log
           )
        {
            var result = await repsUser.GetAsync();

            return result;
        }

        public static string GenerateJWTToken(string userId, string userName, List<DAL.Model.Claim> claims = null)
        {
            var tokenClaims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Name, userName)
                        };

            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    tokenClaims.Add(new Claim(claim.ClaimType, claim.Value));
                }
            }

            var tokenResult = TokenManager.GenerateToken(tokenClaims);

            return tokenResult;
        }
    }
}
