using Microsoft.Extensions.Logging;
using Nexus.Base.CosmosDBRepository;
using Nexus.Base.Identity;
using Playground.Identity.DAL;
using System.Security.Claims;

namespace Playground.Identity.BLL.UserManagement
{
    public class UserManager
    {
        private readonly IUnitOfWork _uow;

        public UserManager(IUnitOfWork uow)
        {
            _uow ??= uow;
        }

        public async Task<DAL.Model.User> CreateUser(DAL.Model.User user,
            ILogger log, string createdBy = null
            )
        {
            return await _uow.UserRepository.CreateAsync(user, null, createdBy, null);
        }

        // ToDo : UserId disini literal User.Id
        public async Task<DAL.Model.User> GetUserById(
            string userId, ILogger log
            )
        {
            var result = (await _uow.UserRepository.GetAsync(
                predicate: p => p.Id == userId)).Items.FirstOrDefault();

            return result;
        }

        public async Task<DAL.Model.User> GetUserByEmail(
            string userEmail, ILogger log
            )
        {
            var result = (await _uow.UserRepository.GetAsync(
                predicate: p => p.Email == userEmail)).Items.FirstOrDefault();

            return result;
        }

        public async Task<PageResult<DAL.Model.User>> GetAllUser(
           string continuationToken, ILogger log
           )
        {
            var result = await _uow.UserRepository.GetAsync();

            return result;
        }

        public string GenerateJWTToken(string userId, string userName, List<DAL.Model.Claim> claims = null)
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
