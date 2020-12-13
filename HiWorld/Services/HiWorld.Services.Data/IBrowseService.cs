﻿namespace HiWorld.Services.Data
{
    using HiWorld.Web.ViewModels.Home;
    using System.Collections.Generic;

    public interface IBrowseService
    {
        IEnumerable<T> GetNewestPosts<T>(int profileId, int pageNumber, int count = 20);

        IEnumerable<ProfileFollowingViewModel> GetFollowing(int profileId);

        int GetPostsCount(int profileId);

        IEnumerable<T> SearchPosts<T>(string searchText, int pageNumber, int count = 20);

        IEnumerable<T> SearchPages<T>(string searchText, int pageNumber, int count = 20);

        IEnumerable<T> SearchProfiles<T>(string searchText, int pageNumber, int count = 20);

        int GetSearchCount(string searchText);
    }
}
