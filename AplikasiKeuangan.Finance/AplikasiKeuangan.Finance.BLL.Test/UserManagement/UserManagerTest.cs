using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Nexus.Base.CosmosDBRepository;
using AplikasiKeuangan.Finance.BLL.UserManagement;
using AplikasiKeuangan.Finance.DAL;
using AplikasiKeuangan.Finance.DAL.Model;
using AplikasiKeuangan.Finance.FrontEndAPI.User.DTO;
using System.Linq.Expressions;

namespace AplikasiKeuangan.Finance.BLL.Test.UserManagement
{
    public class UserManagerTests
    {

        public static List<DAL.Model.User> _ITEMS =
       FileManager.LoadDataFromFile<List<User>>(@"MockData\User.json");

        private static IMapper _mapper { get; set; }
        private static Mock<IUnitOfWork> uow = new Mock<IUnitOfWork>();

        static UserManagerTests()
        {
            if (_mapper == null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<User, UserDTO>();
                });

                _mapper = config.CreateMapper();
            }

            Initiator(uow);
        }

        public static void Initiator(Mock<IUnitOfWork> uow)
        {

            uow.Setup(c => c.UserRepository.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Expression<Func<User, User>>>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<string>()
            )).ReturnsAsync((
                Expression<Func<User, bool>> predicate,
                Func<IQueryable<User>, IOrderedQueryable<User>> orderBy,
                Expression<Func<User, User>> selector,
                bool usePaging,
                string continuationToken,
                int pageSize,
                Dictionary<string, string> pk,
                bool enableCrossPartition,
                bool isDebug,
                TimeSpan? cacheExpiry,
                string cachePrefix
                ) => new PageResult<DAL.Model.User>(
                    item: _ITEMS.ToList(),
                    continuationToken: "",
                    diagnostic: "")
                );

        }


        [Collection("UserManagerTest")]
        public class UserManagerTest
        {

            private readonly ILogger _logger;

            [Fact]
            public async Task GetAllUser_AllUser_Success()
            {
                // <Method Name>_<Testing Purpose>_<Expected Result>
                // arrange
                var svc = new UserManager(uow.Object);
                string ct = null;

                // act
                var actual = (await svc.GetAllUser(ct, _logger));

                var expected = new PageResult<User>(
                    continuationToken: ct,
                    item: _mapper.Map<List<User>>(_ITEMS.OrderByDescending(o => o.Id)),
                    diagnostic: null
                    );

                // assert
                Assert.Equal(expected.Items.Count(), actual.Items.Count());
                Assert.Equal(expected.Items.OrderBy(o => o.Id).FirstOrDefault().Id, actual.Items.OrderBy(o => o.Id).FirstOrDefault().Id);
                Assert.Equal(expected.Items.OrderBy(o => o.Id).LastOrDefault().Id, actual.Items.OrderBy(o => o.Id).LastOrDefault().Id);
            }


            [Fact]
            public async Task GetUserById_ReturnUserWithSelectedId_Success()
            {

                // arrange
                var svc = new UserManager(uow.Object);
                string documentId = "test-57327779-58a9-427a-bbda-535c5a0c6679"; // See the Mock File

                // act
                var actual = (await svc.GetUserById(documentId, _logger));

                var expected = _mapper.Map<User>(_ITEMS.Where(w => w.Id == documentId).FirstOrDefault());

                // assert
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.FullName, actual.FullName);
            }
        }


    }

}
