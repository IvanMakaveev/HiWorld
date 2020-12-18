namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Posts;

    public interface IPostsService
    {
        Task CreateForProfileAsync(int id, CreatePostInputModel input, string path);

        Task CreateForPageAsync(int id, CreatePostInputModel input, string path);

        Task LikePostAsync(int profileId, int id);

        Task DeletePostFromProfileAsync(int profileId, int id);

        Task DeletePostFromPageAsync(int pageId, int id);

        Task DeleteAllPostsFromPageAsync(int pageId);

        Task DeleteAllPostsFromProfileAsync(int profileId);

        Task DeleteAsync(int id);

        bool IsLiked(int postId, int accessorId);

        bool IsOwner(int postId, bool isProfile, int accessorId);

        IEnumerable<T> GetProfilePosts<T>(int profileId, int pageNumber, int count = 20);

        int GetProfileTotalPosts(int profileId);

        IEnumerable<T> GetPagePosts<T>(int pageId, int pageNumber, int count = 20);

        int GetPageTotalPosts(int pageId);

        IEnumerable<T> GetAllPosts<T>();
    }
}
