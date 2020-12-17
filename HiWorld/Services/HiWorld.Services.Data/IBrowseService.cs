namespace HiWorld.Services.Data
{
    using System.Collections.Generic;

    using HiWorld.Web.ViewModels.Home;

    public interface IBrowseService
    {
        IEnumerable<T> GetNewestPosts<T>(int profileId, int pageNumber, int count = 20);

        IEnumerable<ProfileFollowingViewModel> GetFollowing(int profileId);

        int GetPostsCount(int profileId);

        IEnumerable<T> SearchPosts<T>(string[] searchTokens, int pageNumber, int count = 20);

        IEnumerable<T> SearchPages<T>(string[] searchTokens, int pageNumber, int count = 20);

        IEnumerable<T> SearchProfiles<T>(string[] searchTokens, int pageNumber, int count = 20);

        int GetSearchCount(string[] searchTokens);
    }
}
