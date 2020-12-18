namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Posts;

    public interface ICommentsService
    {
        Task<T> AddCommentAsync<T>(int profileId, PostCommentInputModel input);

        Task LikeCommentAsync(int commentId, int profileId);

        Task DeleteAllCommentsFromProfileAsync(int profileId);

        Task DeleteAsync(int id);

        bool IsLiked(int commentId, int accessorId);

        IEnumerable<T> GetAllComments<T>();
    }
}
