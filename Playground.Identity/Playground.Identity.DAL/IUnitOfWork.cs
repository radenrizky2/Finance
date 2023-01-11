using Nexus.Base.CosmosDBRepository;

namespace Playground.Identity.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IDocumentDBRepository<Model.User> UserRepository { get; }
    }
}
