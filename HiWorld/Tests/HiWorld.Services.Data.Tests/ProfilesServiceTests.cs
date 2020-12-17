namespace HiWorld.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data;
    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Data.Models.Enums;
    using HiWorld.Data.Repositories;
    using HiWorld.Services.Data.Tests.FakeModels;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Profiles;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class ProfilesServiceTests : IDisposable
    {
        private IDeletableEntityRepository<Profile> profileRepository;
        private IRepository<Country> countriesRepository;
        private IRepository<ProfileFollower> followersRepository;
        private IImagesService imagesService;
        private IPostsService postsService;
        private ICommentsService commentsService;
        private ApplicationDbContext dbContext;
        private ProfilesService profilesService;

        public ProfilesServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.Create(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Returns(Task.Run(() => "test"));
            this.imagesService = mockImageService.Object;

            var mockPostsService = new Mock<IPostsService>();
            mockPostsService.Setup(x => x.DeleteAllPostsFromProfile(It.IsAny<int>()))
                .Returns(Task.Run(() => { return; }));
            this.postsService = mockPostsService.Object;

            var mockCommnetsService = new Mock<ICommentsService>();
            mockCommnetsService.Setup(x => x.DeleteAllCommentsFromProfile(It.IsAny<int>()))
                .Returns(Task.Run(() => { return; }));
            this.commentsService = mockCommnetsService.Object;

            var connection = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            this.dbContext = new ApplicationDbContext(connection.Options);
            this.dbContext.Database.EnsureCreated();

            this.profileRepository = new EfDeletableEntityRepository<Profile>(this.dbContext);
            this.countriesRepository = new EfRepository<Country>(this.dbContext);
            this.followersRepository = new EfRepository<ProfileFollower>(this.dbContext);
            this.profilesService = new ProfilesService(
                this.profileRepository,
                this.countriesRepository,
                this.followersRepository,
                this.postsService,
                this.commentsService,
                this.imagesService);
        }

        [Fact]
        public async Task GetIdWorskCorrectly()
        {
            await this.SeedData();

            var result = this.profilesService.GetId("test");

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CreateAsyncWorskCorrectly()
        {
            await this.SeedData();

            var input = new BaseInfoInputModel()
            {
                BirthDate = DateTime.Now,
                CountryId = 1,
                FirstName = "test",
                LastName = "test",
                Gender = "Male",
            };

            var result = await this.profilesService.CreateAsync(input);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CreateAsyncThrowsWithInvalidCountry()
        {
            await this.SeedData();

            var input = new BaseInfoInputModel()
            {
                BirthDate = DateTime.Now,
                CountryId = 2,
                FirstName = "test",
                LastName = "test",
                Gender = "Male",
            };

            await Assert.ThrowsAsync<ArgumentException>(() => this.profilesService.CreateAsync(input));
        }

        [Fact]
        public async Task CreateAsyncThrowsWithInvalidGender()
        {
            await this.SeedData();

            var input = new BaseInfoInputModel()
            {
                BirthDate = DateTime.Now,
                CountryId = 1,
                FirstName = "test",
                LastName = "test",
                Gender = "test",
            };

            await Assert.ThrowsAsync<ArgumentException>(() => this.profilesService.CreateAsync(input));
        }

        [Fact]
        public async Task IsFriendWorksCorrectly()
        {
            await this.SeedData();

            var result = this.profilesService.IsFriend(1, 2);
            var secondResult = this.profilesService.IsFriend(1, 3);

            Assert.False(result);
            Assert.True(secondResult);
        }

        [Fact]
        public async Task IsPendingWorksCorrectly()
        {
            await this.SeedData();

            var result = this.profilesService.IsPending(1, 2);
            var secondResult = this.profilesService.IsPending(1, 3);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task IsFollowingWorksCorrectly()
        {
            await this.SeedData();

            var result = this.profilesService.IsFollowing(1, 2);
            var secondResult = this.profilesService.IsFollowing(1, 3);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task FollowProfileAsyncAddsFollowerCorrectly()
        {
            await this.SeedData();

            await this.profilesService.FollowProfileAsync(1, 3);

            Assert.Equal(2, await this.followersRepository.All().CountAsync());
        }

        [Fact]
        public async Task FollowProfileAsyncRemovesFollowerCorrectly()
        {
            await this.SeedData();

            await this.profilesService.FollowProfileAsync(1, 2);

            Assert.Equal(0, await this.followersRepository.All().CountAsync());
        }

        [Fact]
        public async Task GetByIdWorksCorrectly()
        {
            await this.SeedData();

            var result = this.profilesService.GetById<FakeProfileModel>(1);

            Assert.Equal("test", result.FirstName);
        }

        [Fact]
        public async Task GetByUserIdWorksCorrectly()
        {
            await this.SeedData();

            var result = this.profilesService.GetByUserId<FakeProfileModel>("test");

            Assert.Equal("test", result.FirstName);
        }

        [Fact]
        public async Task UpdateAsyncDoesntUpdateWithNonExistentId()
        {
            await this.SeedData();

            var input = new EditProfileInputModel();
            await this.profilesService.UpdateAsync("test2", input, "test");

            Assert.Equal("test", (await this.profileRepository.All().FirstOrDefaultAsync()).FirstName);
        }

        [Fact]
        public async Task UpdateAsyncWorksCorrectly()
        {
            await this.SeedData();

            var input = new EditProfileInputModel()
            {
                FirstName = "test2",
                Image = null,
                Gender = "Male",
                CountryId = 1,
            };

            await this.profilesService.UpdateAsync("test", input, "test");

            Assert.Equal("test2", (await this.profileRepository.All().FirstOrDefaultAsync()).FirstName);
        }

        [Fact]
        public async Task UpdateAsyncWorksCorrectlyWithImage()
        {
            await this.SeedData();

            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);
            var input = new EditProfileInputModel()
            {
                FirstName = "test2",
                Image = imageMock.Object,
                Gender = "Male",
                CountryId = 1,
            };

            await this.profilesService.UpdateAsync("test", input, "test");

            Assert.Equal("test2", (await this.profileRepository.All().FirstOrDefaultAsync()).FirstName);
        }

        [Fact]
        public async Task DeleteAsyncWorksCorrectly()
        {
            await this.SeedData();

            await this.profilesService.DeleteAsync(1);

            Assert.Equal(0, await this.profileRepository.All().CountAsync());
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        private async Task SeedData()
        {
            await this.countriesRepository.AddAsync(new Country() { Name = "test" });
            await this.countriesRepository.SaveChangesAsync();

            await this.profileRepository.AddAsync(new Profile()
            {
                FirstName = "test",
                LastName = "test",
                CountryId = 1,
                BirthDate = DateTime.UtcNow,
                User = new ApplicationUser()
                {
                    Id = "test",
                },
                FriendsRecieved = new List<ProfileFriend>() { new ProfileFriend() { IsAccepted = false, ProfileId = 2 } },
                FriendsSent = new List<ProfileFriend>() { new ProfileFriend() { IsAccepted = true, FriendId = 3 } },
            });
            await this.profileRepository.SaveChangesAsync();

            await this.followersRepository.AddAsync(new ProfileFollower
            {
                ProfileId = 1,
                FollowerId = 2,
            });
            await this.followersRepository.SaveChangesAsync();
        }
    }
}
