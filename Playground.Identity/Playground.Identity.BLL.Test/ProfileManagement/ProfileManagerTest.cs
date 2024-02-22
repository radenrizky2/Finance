using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Nexus.Base.CosmosDBRepository;
using Playground.Identity.BLL.ProfileManagement;
using Playground.Identity.DAL;
using System.Linq.Expressions;

namespace Playground.Identity.BLL.Test.ProfileManagement
{
    public class ProfileManagerTests
    {
        public static List<DAL.Model.Profile> _PROFILES =
            FileManager.LoadDataFromFile<List<DAL.Model.Profile>>(@"MockData\Profile.json");

        private static IMapper _mapper { get; set; }
        private static Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();

        static ProfileManagerTests()
        {
            if (_mapper == null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    // Adjust mapping configuration as needed
                    cfg.CreateMap<DAL.Model.Profile, DAL.Model.Profile>();
                });

                _mapper = config.CreateMapper();
            }

            InitMockRepository(uow);
        }

        private static void InitMockRepository(Mock<IUnitOfWork> uow)
        {
            // Mock the ProfileRepository's GetAsync method
            uow.Setup(c => c.ProfileRepository.GetAsync(
                It.IsAny<Expression<Func<DAL.Model.Profile, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Model.Profile>, IOrderedQueryable<DAL.Model.Profile>>>(),
                It.IsAny<Expression<Func<DAL.Model.Profile, DAL.Model.Profile>>>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<string>()
            )).ReturnsAsync((
                Expression<Func<DAL.Model.Profile, bool>> predicate,
                Func<IQueryable<DAL.Model.Profile>, IOrderedQueryable<DAL.Model.Profile>> orderBy,
                Expression<Func<DAL.Model.Profile, DAL.Model.Profile>> selector,
                bool usePaging,
                string continuationToken,
                int pageSize,
                Dictionary<string, string> pk,
                bool enableCrossPartition,
                bool isDebug,
                TimeSpan? cacheExpiry,
                string cachePrefix
                ) => new PageResult<DAL.Model.Profile>(
                    _PROFILES.ToList(), // Adjust based on your mock data and filtering
                    continuationToken: "",
                    diagnostic: "")
                );
        }

        [Collection("ProfileManagerTest")]
        public class ProfileManagerTest
        {
            private readonly ILogger _logger;

            [Fact]
            public async Task GetProfileData_ReturnsProfiles_Success()
            {
                // Arrange
                var svc = new ProfileManager(uow.Object);
                var requestDTO = new RequestDTO
                {
                    // Populate with relevant request data
                    FullName = "John Doe",
                    City = "New York",
                    ContinuationToken = null
                };

                // Act
                var actual = await svc.GetProfileData(requestDTO);

                // Assert
                // Customize assertions based on expected behavior and mock data
                Assert.NotNull(actual);
                Assert.True(actual.Items.Any()); // Ensure some items are returned
                // Further assertions can check for specific data points as needed
            }
        }
    }
}
