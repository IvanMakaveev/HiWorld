namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Posts;

    public interface ICommentsService
    {
        Task<T> AddCommentAsync<T>(int profileId, PostCommentInputModel input);

        Task LikeCommentAsync(int commentId, int profileId);

        Task DeleteAllCommentsFromProfile(int profileId);

        bool IsLiked(int commentId, int accessorId);
    }
}
