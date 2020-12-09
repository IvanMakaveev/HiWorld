namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Posts;

    public interface IPostsService
    {
        Task CreateForProfile(int id, CreatePostInputModel input, string path);

        Task LikePost(int profileId, int id);

        Task DeletePostFromProfile(int profileId, int id);

        Task<T> AddComment<T>(int profileId, PostCommentInputModel input);

        bool IsLiked(int postId, string userId);
    }
}
