namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Posts;

    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;
        private readonly IRepository<CommentLike> commentLikesRepository;

        public CommentsService(
            IDeletableEntityRepository<Comment> commentsRepository,
            IRepository<CommentLike> commentLikesRepository)
        {
            this.commentsRepository = commentsRepository;
            this.commentLikesRepository = commentLikesRepository;
        }

        public async Task<T> AddCommentAsync<T>(int profileId, PostCommentInputModel input)
        {
            var comment = new Comment()
            {
                PostId = input.PostId,
                ProfileId = profileId,
                Text = input.Text,
            };

            await this.commentsRepository.AddAsync(comment);
            await this.commentsRepository.SaveChangesAsync();

            return this.commentsRepository.AllAsNoTracking().Where(x => x.Id == comment.Id).To<T>().FirstOrDefault();
        }

        public async Task DeleteAllCommentsFromProfileAsync(int profileId)
        {
            var comments = this.commentsRepository.All().Where(x => x.ProfileId == profileId).ToList();
            foreach (var comment in comments)
            {
                this.commentsRepository.Delete(comment);
            }

            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = this.commentsRepository.All().Where(x => x.Id == id).FirstOrDefault();
            if (comment != null)
            {
                this.commentsRepository.Delete(comment);
                await this.commentsRepository.SaveChangesAsync();
            }
        }

        public IEnumerable<T> GetAllComments<T>()
            => this.commentsRepository
            .AllAsNoTracking()
            .To<T>()
            .ToList();

        public bool IsLiked(int commentId, int accessorId)
            => this.commentLikesRepository
            .AllAsNoTracking()
            .Any(x => x.CommentId == commentId && x.ProfileId == accessorId);

        public async Task LikeCommentAsync(int commentId, int profileId)
        {
            var commentLike = this.commentLikesRepository.All().Where(x => x.CommentId == commentId && x.ProfileId == profileId).FirstOrDefault();
            if (commentLike == null)
            {
                commentLike = new CommentLike()
                {
                    ProfileId = profileId,
                    CommentId = commentId,
                };

                await this.commentLikesRepository.AddAsync(commentLike);
            }
            else
            {
                this.commentLikesRepository.Delete(commentLike);
            }

            await this.commentLikesRepository.SaveChangesAsync();
        }
    }
}
