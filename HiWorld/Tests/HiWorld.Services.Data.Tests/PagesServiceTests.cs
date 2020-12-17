using HiWorld.Data;
using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Data.Repositories;
using HiWorld.Services.Data.Tests.FakeModels;
using HiWorld.Services.Mapping;
using HiWorld.Web.ViewModels.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HiWorld.Services.Data.Tests
{
    public class PagesServiceTests : IDisposable
    {
        private IDeletableEntityRepository<Page> pagesRepository;
        private IRepository<PageFollower> followersRepository;
        private IRepository<PageTag> pageTagsRepository;
        private ITagsService tagsService;
        private IImagesService imagesService;
        private IPostsService postsService;
        private ApplicationDbContext dbContext;
        private PagesService pagesService;

        public PagesServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.Create(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Returns(Task.Run(() => "test"));
            this.imagesService = mockImageService.Object;

            var mockTagService = new Mock<ITagsService>();
            mockTagService.Setup(x => x.GetIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => 2));
            this.tagsService = mockTagService.Object;

            var mockPostService = new Mock<IPostsService>();
            mockPostService.Setup(x => x.DeleteAllPostsFromPage(It.IsAny<int>()))
                .Returns(Task.Run(() => { return; }));
            this.postsService = mockPostService.Object;

            var connection = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            this.dbContext = new ApplicationDbContext(connection.Options);
            this.dbContext.Database.EnsureCreated();

            this.pagesRepository = new EfDeletableEntityRepository<Page>(this.dbContext);
            this.followersRepository = new EfRepository<PageFollower>(this.dbContext);
            this.pageTagsRepository = new EfRepository<PageTag>(this.dbContext);
            this.pagesService = new PagesService(
                this.pagesRepository,
                this.followersRepository,
                this.pageTagsRepository,
                this.tagsService,
                this.imagesService,
                this.postsService);
        }

        [Fact]
        public async Task CreateAsyncWorksCorrectly()
        {
            var input = new CreatePageInputModel
            {
                Name = "test",
                Tags = null,
                Image = null,
            };

            await this.pagesService.CreateAsync(1, input, "test");

            Assert.Equal(1, await this.pagesRepository.All().CountAsync());
        }

        [Fact]
        public async Task CreateAsyncWorksCorrectlyWithImageAndTags()
        {
            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);
            var input = new CreatePageInputModel
            {
                Name = "test",
                Tags = new List<string>() { "tag1", "tag2" },
                Image = imageMock.Object,
            };

            await this.pagesService.CreateAsync(1, input, "test");

            Assert.Equal(1, await this.pagesRepository.All().CountAsync());
        }

        [Fact]
        public async Task GetByIdWorksCorrectly()
        {
            await this.SeedData();

            var result = this.pagesService.GetById<FakePageModel>(1);

            Assert.Equal("test", result.Name);
        }

        [Fact]
        public async Task GetProfilePagesWorksCorrectly()
        {
            await this.SeedData();

            var result = this.pagesService.GetProfilePages<FakePageModel>(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task IsFollowingWorksCorrectly()
        {
            await this.SeedData();

            var result = this.pagesService.IsFollowing(2, 1);
            var secondResult = this.pagesService.IsFollowing(1, 1);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task IsOwnerWithProfileIdWorksCorrectly()
        {
            await this.SeedData();

            var result = this.pagesService.IsOwner(1, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsOwnerWithUserIdWorksCorrectly()
        {
            await this.SeedData();

            var result = this.pagesService.IsOwner("test", 1);

            Assert.True(result);
        }

        [Fact]
        public async Task FollowPageDoesntAddFollowerIfOwner()
        {
            await this.SeedData();

            await this.pagesService.FollowPageAsync(1, 1);
            Assert.Equal(1, await this.followersRepository.All().CountAsync());
        }

        [Fact]
        public async Task FollowPageAddsFollowerCorrectly()
        {
            await this.SeedData();

            await this.pagesService.FollowPageAsync(3, 1);
            Assert.Equal(2, await this.followersRepository.All().CountAsync());
        }

        [Fact]
        public async Task FollowPageRemovesFollowerCorrectly()
        {
            await this.SeedData();

            await this.pagesService.FollowPageAsync(3, 1);
            await this.pagesService.FollowPageAsync(3, 1);

            Assert.Equal(1, await this.followersRepository.All().CountAsync());
        }

        [Fact]
        public async Task UpdateAsyncDoesntUpdateWithNonExistendId()
        {
            await this.SeedData();

            var input = new EditPageInputModel()
            {
                Id = 2,
                Name = "test2",
            };

            await this.pagesService.UpdateAsync(input, "test");

            Assert.Equal("test", (await this.pagesRepository.All().FirstOrDefaultAsync()).Name);
        }

        [Fact]
        public async Task UpdateAsyncWorksCorrectly()
        {
            await this.SeedData();

            var input = new EditPageInputModel()
            {
                Id = 1,
                Name = "test2",
                Tags = new List<string> { "tag1" },
            };

            await this.pagesService.UpdateAsync(input, "test");

            Assert.Equal("test2", (await this.pagesRepository.All().FirstOrDefaultAsync()).Name);
        }

        [Fact]
        public async Task UpdateAsyncWorksCorrectlyWithImageAndTags()
        {
            await this.SeedData();

            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);

            var input = new EditPageInputModel()
            {
                Id = 1,
                Name = "test2",
                Tags = new List<string> { "tag2" },
                Image = imageMock.Object,
            };

            await this.pagesService.UpdateAsync(input, "test");

            Assert.Equal("test2", (await this.pagesRepository.All().FirstOrDefaultAsync()).Name);
        }

        [Fact]
        public async Task UpdateAsyncWorksCorrectlyWithImageAndNoTags()
        {
            await this.SeedData();

            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);

            var input = new EditPageInputModel()
            {
                Id = 1,
                Name = "test2",
                Tags = null,
                Image = imageMock.Object,
            };

            await this.pagesService.UpdateAsync(input, "test");

            Assert.Equal("test2", (await this.pagesRepository.All().FirstOrDefaultAsync()).Name);
        }

        [Fact]
        public async Task DeleteAsyncWorksCorrectly()
        {
            await this.SeedData();

            await this.pagesService.DeleteAsync(1);

            Assert.Equal(0, await this.pagesRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeleteAsyncDoesntDeleteNonExitentId()
        {
            await this.SeedData();

            await this.pagesService.DeleteAsync(2);

            Assert.Equal(1, await this.pagesRepository.All().CountAsync());
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        private async Task SeedData()
        {
            await this.pagesRepository.AddAsync(new Page()
            {
                Id = 1,
                ProfileId = 1,
                Name = "test",
                Profile = new Profile() { User = new ApplicationUser { Id = "test" } },
            });

            await this.pagesRepository.SaveChangesAsync();

            await this.followersRepository.AddAsync(new PageFollower() { FollowerId = 2, PageId = 1 });
            await this.followersRepository.SaveChangesAsync();

            await this.pageTagsRepository.AddAsync(new PageTag() { PageId = 1, TagId = 1, Tag = new Tag() { Id = 1, Name = "tag1" } });
            await this.pageTagsRepository.SaveChangesAsync();
        }
    }
}
