namespace HiWorld.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data;
    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Data.Repositories;
    using HiWorld.Services.Data.Tests.FakeModels;
    using HiWorld.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class BrowseServiceTests : IDisposable
    {
        private IDeletableEntityRepository<Page> pageRepository;
        private IDeletableEntityRepository<Post> postsRepository;
        private IDeletableEntityRepository<Profile> profilesRepository;
        private IRepository<ProfileFollower> profileFollowersRepository;
        private IRepository<PageFollower> pageFollowersRepository;
        private ApplicationDbContext dbContext;
        private BrowseService browseService;

        public BrowseServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var connection = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            this.dbContext = new ApplicationDbContext(connection.Options);
            this.dbContext.Database.EnsureCreated();

            this.pageRepository = new EfDeletableEntityRepository<Page>(this.dbContext);
            this.postsRepository = new EfDeletableEntityRepository<Post>(this.dbContext);
            this.profilesRepository = new EfDeletableEntityRepository<Profile>(this.dbContext);
            this.profileFollowersRepository = new EfRepository<ProfileFollower>(this.dbContext);
            this.pageFollowersRepository = new EfRepository<PageFollower>(this.dbContext);
            this.browseService = new BrowseService(
                this.pageRepository,
                this.postsRepository,
                this.profilesRepository,
                this.profileFollowersRepository,
                this.pageFollowersRepository);
        }

        [Fact]
        public async Task GetFollowingWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.GetFollowing(1);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetNewestPostsWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.GetNewestPosts<FakePostModel>(1, 1);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPostsCountWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.GetPostsCount(1);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetSearchCountWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.GetSearchCount(new string[] { "test" });

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task SearchPagesWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.SearchPages<FakePageModel>(new string[] { "test" }, 1);

            Assert.Single(result);
            Assert.Equal("test", result.FirstOrDefault().Name);
        }

        [Fact]
        public async Task SearchPostsWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.SearchPosts<FakePostModel>(new string[] { "test" }, 1);

            Assert.Equal(2, result.Count());
            Assert.Equal("test", result.FirstOrDefault().Text);
        }

        [Fact]
        public async Task SearchProfilesWorksCorrectly()
        {
            await this.SeedData();

            var result = this.browseService.SearchProfiles<FakeProfileModel>(new string[] { "test" }, 1);

            Assert.Equal(2, result.Count());
            Assert.Equal("test", result.FirstOrDefault().FirstName);
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        private async Task SeedData()
        {
            await this.pageRepository.AddAsync(new Page()
            {
                Name = "test",
                Posts = new List<Post>() { new Post() { Text = "test" } },
            });

            await this.pageRepository.SaveChangesAsync();

            await this.profilesRepository.AddAsync(new Profile()
            {
                FirstName = "test",
                LastName = "test",
            });
            await this.profilesRepository.AddAsync(new Profile()
            {
                FirstName = "test2",
                LastName = "test2",
                Posts = new List<Post>() { new Post() { Text = "test" } },
            });
            await this.profilesRepository.SaveChangesAsync();

            await this.pageFollowersRepository.AddAsync(new PageFollower()
            {
                PageId = 1,
                FollowerId = 1,
            });
            await this.pageFollowersRepository.SaveChangesAsync();

            await this.profileFollowersRepository.AddAsync(new ProfileFollower()
            {
                ProfileId = 2,
                FollowerId = 1,
            });
            await this.profileFollowersRepository.SaveChangesAsync();
        }
    }
}
