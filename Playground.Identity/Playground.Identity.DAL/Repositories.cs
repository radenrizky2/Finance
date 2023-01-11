using Nexus.Base.CosmosDBRepository;
using Microsoft.Azure.Cosmos;

namespace Playground.Identity.DAL
{
    public class Repositories
    {
        // To Access Eventgrid
        private static readonly string C_EventGridEndPoint = Environment.GetEnvironmentVariable("EventGridEndPoint");
        private static readonly string C_EventGridKey = Environment.GetEnvironmentVariable("EventGridKey");

        public class UserRepository : DocumentDBRepository<DAL.Model.User>
        {
            public UserRepository(CosmosClient client) :
                base(databaseId: "Identity", cosmosDBClient: client)
            { }
        }
    }
}
