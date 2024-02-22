using Nexus.Base.CosmosDBRepository;
using Playground.Identity.DAL;

namespace Playground.Identity.BLL.ProfileManagement
{
    public class ProfileManager
    {

        private readonly IUnitOfWork _uow;

        public ProfileManager(IUnitOfWork uow)
        {
            _uow ??= uow;
        }

        public async Task<PageResult<DAL.Model.Profile>> GetProfileData(RequestDTO requestDTO)
        {
            var resultDB = await _uow.ProfileRepository.GetAsync(
                selector: s => new DAL.Model.Profile()
                {
                    FullName = s.FullName,
                    City = s.City,
                    Edisi = s.Edisi,
                    Id = s.Id,
                    ActiveFlag = s.ActiveFlag,
                },
                orderBy: ob => ob.OrderByDescending(o => o.Edisi),
                predicate: p => p.FullName == requestDTO.FullName,
                partitionKeys: new Dictionary<string, string>() { { "city", requestDTO.City } },
                continuationToken: requestDTO.ContinuationToken,
                //enableCrossPartition: true
                //cachePrefix: $"Profile-{city}-{name}",
                //cacheExpiry: new TimeSpan(00, 05, 00)
                isDebug: true,
                //pageNumber: 2
                usePaging: true,
                pageSize: 2
                );

            var continuationToken = resultDB.ContinuationToken;
            var result = resultDB.Items.ToList();
            return resultDB;
        }
    }
}
