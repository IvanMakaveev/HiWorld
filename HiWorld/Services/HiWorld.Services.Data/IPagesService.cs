namespace HiWorld.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Pages;

    public interface IPagesService
    {
        T GetById<T>(int id);

        IEnumerable<T> GetProfilePages<T>(int profileId);

        Task<int> CreateAsync(int profileId, CreatePageInputModel input, string path);

        bool IsOwner(int profileId, int pageId);

        bool IsOwner(string userId, int pageId);

        bool IsFollowing(int profileId, int pageId);

        Task FollowPageAsync(int profileId, int pageId);

        Task DeleteAsync(int pageId);

        Task UpdateAsync(EditPageInputModel input, string path);

        IEnumerable<T> GetAllPages<T>();
    }
}
