namespace HiWorld.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Services.Data.Tests.FakeModels;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Posts;
    using Moq;
    using Xunit;

    public class CommentsServiceTests
    {
        private List<Comment> commentRepoStorage = new List<Comment>();
        private List<CommentLike> commentLikeRepoStorage = new List<CommentLike>();
        private IDeletableEntityRepository<Comment> commentsRepository;
        private IRepository<CommentLike> commentLikesRepository;
        private CommentsService commentsService;

        public CommentsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockCommentLikesRepo = new Mock<IRepository<CommentLike>>();
            mockCommentLikesRepo.Setup(x => x.AddAsync(It.IsAny<CommentLike>()))
                .Callback((CommentLike commentLike) => this.commentLikeRepoStorage.Add(commentLike));
            mockCommentLikesRepo.Setup(x => x.All())
                .Returns(this.commentLikeRepoStorage.AsQueryable);
            mockCommentLikesRepo.Setup(x => x.AllAsNoTracking())
                .Returns(this.commentLikeRepoStorage.AsQueryable);
            mockCommentLikesRepo.Setup(x => x.Delete(It.IsAny<CommentLike>()))
                .Callback((CommentLike commentLike) => this.commentLikeRepoStorage.Remove(commentLike));
            this.commentLikesRepository = mockCommentLikesRepo.Object;

            var mockCommentsRepo = new Mock<IDeletableEntityRepository<Comment>>();
            mockCommentsRepo.Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Callback((Comment commentLike) => this.commentRepoStorage.Add(commentLike));
            mockCommentsRepo.Setup(x => x.All())
                .Returns(this.commentRepoStorage.AsQueryable);
            mockCommentsRepo.Setup(x => x.Delete(It.IsAny<Comment>()))
                .Callback((Comment comment) => this.commentRepoStorage.Remove(comment));

            this.commentsRepository = mockCommentsRepo.Object;

            this.commentsService = new CommentsService(this.commentsRepository, this.commentLikesRepository);
        }

        [Fact]
        public async Task AddCommentAsyncWorksCorrectly()
        {
            await this.commentsService
                .AddCommentAsync<FakeCommentModel>(1, new PostCommentInputModel { PostId = 1, Text = "test" });

            Assert.Single(this.commentRepoStorage);
        }

        [Fact]
        public void IsLikedWorksCorrectly()
        {
            this.commentLikeRepoStorage.Add(new CommentLike
            {
                ProfileId = 1,
                CommentId = 1,
            });

            var result = this.commentsService.IsLiked(1, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task LikeCommentAsyncWorksCorrectly()
        {
            this.commentLikeRepoStorage.Add(new CommentLike
            {
                ProfileId = 1,
                CommentId = 1,
            });

            await this.commentsService.LikeCommentAsync(1, 1);
            await this.commentsService.LikeCommentAsync(1, 2);
            await this.commentsService.LikeCommentAsync(1, 3);

            Assert.Equal(2, this.commentLikeRepoStorage.Count);
        }

        [Fact]
        public async Task DeleteAllCommentsFromProfileWorksCorrectly()
        {
            this.commentRepoStorage.Add(new Comment
            {
                ProfileId = 1,
            });
            this.commentRepoStorage.Add(new Comment
            {
                ProfileId = 1,
            });

            await this.commentsService.DeleteAllCommentsFromProfile(1);

            Assert.Empty(this.commentRepoStorage);
        }

        [Fact]
        public async Task DeleteAllCommentsFromDoesntDeleteWithNonExistentId()
        {
            this.commentRepoStorage.Add(new Comment
            {
                ProfileId = 1,
            });
            this.commentRepoStorage.Add(new Comment
            {
                ProfileId = 1,
            });

            await this.commentsService.DeleteAllCommentsFromProfile(2);

            Assert.Equal(2, this.commentRepoStorage.Count());
        }
    }
}
