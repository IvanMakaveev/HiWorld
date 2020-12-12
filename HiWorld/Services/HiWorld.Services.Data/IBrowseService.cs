namespace HiWorld.Services.Data
{
    using HiWorld.Web.ViewModels.Home;
    using System.Collections.Generic;

    public interface IBrowseService
    {
        IEnumerable<T> GetNewestPosts<T>(int profileId);

        IEnumerable<ProfileFollowingViewModel> GetFollowing(int profileId);
    }
}
