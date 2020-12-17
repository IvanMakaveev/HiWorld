using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Services.Data.Tests.FakeModels;
using HiWorld.Services.Mapping;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HiWorld.Services.Data.Tests
{
    public class TagsServiceTests
    {
        private List<Tag> repoStorage = new List<Tag>();
        private IRepository<Tag> tagsRepo;
        private TagsService tagsService;

        public TagsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockRepo = new Mock<IRepository<Tag>>();
            mockRepo.Setup(x => x.AddAsync(It.IsAny<Tag>()))
                .Callback((Tag tag) => this.repoStorage.Add(tag));
            mockRepo.Setup(x => x.AllAsNoTracking())
                .Returns(this.repoStorage.AsQueryable);
            mockRepo.Setup(x => x.All())
                .Returns(this.repoStorage.AsQueryable);

            this.tagsRepo = mockRepo.Object;
            this.tagsService = new TagsService(this.tagsRepo);
        }

        [Fact]
        public async Task GetTagIdDoesntCreateTagIfAlreadyExists()
        {
            var name = "text";
            this.repoStorage.Add(new Tag()
            {
                Id = 1,
                Name = name,
            });

            var result = await this.tagsService.GetIdAsync(name);

            Assert.Equal(1, result);
            Assert.Single(this.repoStorage);
        }

        [Fact]
        public async Task GetTagIdCreatesTagIfNonExistent()
        {
            var name = "text";

            var result = await this.tagsService.GetIdAsync(name);

            Assert.Single(this.repoStorage);
        }

        [Fact]
        public void GetNameWorksCorrectly()
        {
            var name = "text";
            this.repoStorage.Add(new Tag()
            {
                Id = 1,
                Name = name,
            });

            var result = this.tagsService.GetName(1);

            Assert.Equal(name, result);
        }

        [Fact]
        public void SearchPagesByTagWorksCorrectly()
        {
            string pageName = "testPage";

            this.repoStorage.Add(new Tag()
            {
                Id = 1,
                Name = "test",
                PageTags = new List<PageTag>()
                {
                    new PageTag()
                    {
                        Id = 1,
                        Page = new Page() {Id = 1, Name = pageName},
                    },
                },
            });

            var result = this.tagsService.SearchPagesByTag<FakePageModel>(1, 1);

            Assert.Single(result);
            Assert.Equal(pageName, result.First().Name);
        }

        [Fact]
        public void SearchPagesByTagCountWorksCorrectly()
        {
            this.repoStorage.Add(new Tag()
            {
                Id = 1,
                Name = "test",
                PageTags = new List<PageTag>()
                {
                    new PageTag()
                    {
                        Id = 1,
                        Page = new Page(),
                    },
                    new PageTag()
                    {
                        Id = 2,
                        Page = new Page(),
                    },
                },
            });

            var result = this.tagsService.SearchPagesByTagCount(1);

            Assert.Equal(2, result);
        }

        [Fact]
        public void SearchPostsByTagWorksCorrectly()
        {
            string postText = "testText";

            this.repoStorage.Add(new Tag()
            {
                Id = 1,
                Name = "test",
                PostTags = new List<PostTag>()
                {
                    new PostTag()
                    {
                        Id = 1,
                        Post = new Post() {Id = 1, Text = postText},
                    },
                },
            });

            var result = this.tagsService.SearchPostsByTag<FakePostModel>(1, 1);

            Assert.Single(result);
            Assert.Equal(postText, result.First().Text);
        }

        [Fact]
        public void SearchPostsByTagCountWorksCorrectly()
        {
            this.repoStorage.Add(new Tag()
            {
                Id = 1,
                Name = "test",
                PostTags = new List<PostTag>()
                {
                    new PostTag()
                    {
                        Id = 1,
                        Post = new Post(),
                    },
                    new PostTag()
                    {
                        Id = 2,
                        Post = new Post(),
                    },
                },
            });

            var result = this.tagsService.SearchPostsByTagCount(1);

            Assert.Equal(2, result);
        }
    }
}
