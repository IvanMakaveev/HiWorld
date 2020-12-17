using HiWorld.Data;
using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Data.Repositories;
using HiWorld.Services.Data.Tests.FakeModels;
using HiWorld.Services.Mapping;
using HiWorld.Web.ViewModels.Posts;
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
    public class PostsServiceTests : IDisposable
    {
        private IDeletableEntityRepository<Post> postsRepository;
        private IRepository<PostTag> postTagsRepository;
        private IRepository<PostLike> postLikesRepository;
        private IImagesService imagesService;
        private ITagsService tagsService;
        private PostsService postsService;
        private ApplicationDbContext dbContext;

        public PostsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.Create(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Returns(Task.Run(() => "test"));
            this.imagesService = mockImageService.Object;

            var mockTagService = new Mock<ITagsService>();
            mockTagService.Setup(x => x.GetIdAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => 1));
            this.tagsService = mockTagService.Object;

            var connection = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            this.dbContext = new ApplicationDbContext(connection.Options);
            this.dbContext.Database.EnsureCreated();

            this.postsRepository = new EfDeletableEntityRepository<Post>(this.dbContext);
            this.postTagsRepository = new EfRepository<PostTag>(this.dbContext);
            this.postLikesRepository = new EfRepository<PostLike>(this.dbContext);
            this.postsService = new PostsService(
                this.postsRepository,
                this.postTagsRepository,
                this.postLikesRepository,
                this.imagesService,
                this.tagsService);
        }

        [Fact]
        public async Task CreateForProfileAsyncWorksCorrectly()
        {
            var input = new CreatePostInputModel
            {
                Text = "test",
                Tags = null,
                Image = null,
            };

            await this.postsService.CreateForProfileAsync(1, input, "test");

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task CreateForProfileAsyncWorksCorrectlyWithImageAndTags()
        {
            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);
            var input = new CreatePostInputModel
            {
                Text = "test",
                Tags = new List<string>() { "tag1", "tag2" },
                Image = imageMock.Object,
            };

            await this.postsService.CreateForProfileAsync(1, input, "test");

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task CreateForPageAsyncWorksCorrectly()
        {
            var input = new CreatePostInputModel
            {
                Text = "test",
                Tags = null,
                Image = null,
            };

            await this.postsService.CreateForPageAsync(1, input, "test");

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task CreateForPageAsyncWorksCorrectlyWithImageAndTags()
        {
            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);
            var input = new CreatePostInputModel
            {
                Text = "test",
                Tags = new List<string>() { "tag1", "tag2" },
                Image = imageMock.Object,
            };

            await this.postsService.CreateForPageAsync(1, input, "test");

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeletePostFromProfileAsyncWorksCorrectly()
        {
            await this.SeedData();

            await this.postsService.DeletePostFromProfileAsync(2, 2);

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeletePostFromProfileAsyncDoesNotDeleteIfNonExitentId()
        {
            await this.SeedData();

            await this.postsService.DeletePostFromProfileAsync(3, 2);

            Assert.Equal(2, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeletePostFromPageAsyncWorksCorrectly()
        {
            await this.SeedData();

            await this.postsService.DeletePostFromPageAsync(1, 1);

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeletePostFromPageAsyncDoesNotDeleteIfNonExitentId()
        {
            await this.SeedData();

            await this.postsService.DeletePostFromPageAsync(3, 2);

            Assert.Equal(2, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeleteAllPostsFromPageWorksCorrectly()
        {
            await this.SeedData();

            await this.postsService.DeleteAllPostsFromPage(1);

            Assert.Equal(1, await this.postsRepository.All().CountAsync());
        }

        [Fact]
        public async Task LikePostAsyncAddsLikeCorrectly()
        {
            await this.SeedData();

            await this.postsService.LikePostAsync(1, 1);
            Assert.Equal(2, await this.postLikesRepository.All().CountAsync());
        }

        [Fact]
        public async Task LikePostAsyncRemovesLikeCorrectly()
        {
            await this.SeedData();

            await this.postsService.LikePostAsync(1, 1);
            await this.postsService.LikePostAsync(1, 1);

            Assert.Equal(1, await this.postLikesRepository.All().CountAsync());
        }

        [Fact]
        public async Task IsLikedWorksCorrectly()
        {
            await this.SeedData();

            var result = this.postsService.IsLiked(2, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetProfilePostsWorksCorrectly()
        {
            await this.SeedData();

            var result = this.postsService.GetProfilePosts<FakePostModel>(2, 1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetPagePostsWorksCorrectly()
        {
            await this.SeedData();

            var result = this.postsService.GetPagePosts<FakePostModel>(1, 1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetProfilePostsCountWorksCorrectly()
        {
            await this.SeedData();

            var result = this.postsService.GetProfileTotalPosts(2);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetPagePostsCountWorksCorrectly()
        {
            await this.SeedData();

            var result = this.postsService.GetPageTotalPosts(1);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task IsOwnerWorksCorrectlyForProfile()
        {
            await this.SeedData();

            var result = this.postsService.IsOwner(2, true, 2);
            var secondResult = this.postsService.IsOwner(3, true, 1);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task IsOwnerWorksCorrectlyForPage()
        {
            await this.SeedData();

            var result = this.postsService.IsOwner(1, false, 1);
            var secondResult = this.postsService.IsOwner(3, false, 2);

            Assert.True(result);
            Assert.False(secondResult);
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        private async Task SeedData()
        {
            await this.postsRepository.AddAsync(new Post()
            {
                Id = 1,
                PageId = 1,
            });

            await this.postsRepository.AddAsync(new Post()
            {
                Id = 2,
                ProfileId = 2,
            });

            await this.postsRepository.SaveChangesAsync();

            await this.postLikesRepository.AddAsync(new PostLike() { ProfileId = 1, PostId = 2 });
            await this.postLikesRepository.SaveChangesAsync();
        }
    }
}
