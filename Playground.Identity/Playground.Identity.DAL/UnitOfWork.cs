using Microsoft.Azure.Cosmos;
using Nexus.Base.CosmosDBRepository;

namespace Playground.Identity.DAL
{
    public class UnitOfWork : IUnitOfWork
    {

        private static readonly string C_EventGridEndPoint = Environment.GetEnvironmentVariable("EventGridEndPoint");
        private static readonly string C_EventGridKey = Environment.GetEnvironmentVariable("EventGridKey");
        private static readonly string _DB1 = "Identity";
        private readonly CosmosClient _client;

        private readonly Lazy<IDocumentDBRepository<Model.User>> userRepository;

       
        public UnitOfWork(CosmosClient client)
        {
            _client = client;

            userRepository ??= new Lazy<IDocumentDBRepository<Model.User>>(new DocumentDBRepository<DAL.Model.User>
                (_DB1, _client, eventGridEndPoint: C_EventGridEndPoint, eventGridKey: C_EventGridKey));

        }

        public IDocumentDBRepository<Model.User> UserRepository => userRepository.Value;


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
