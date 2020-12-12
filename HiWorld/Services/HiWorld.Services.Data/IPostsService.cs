namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Posts;

    public interface IPostsService
    {
        Task CreateForProfileAsync(int id, CreatePostInputModel input, string path);

        Task CreateForPageAsync(int id, CreatePostInputModel input, string path);

        Task LikePostAsync(int profileId, int id);

        Task DeletePostFromProfileAsync(int profileId, int id);

        bool IsLiked(int postId, int accessorId);
    }
}
