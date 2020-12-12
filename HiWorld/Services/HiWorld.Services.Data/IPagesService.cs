namespace HiWorld.Services.Data
{
    using HiWorld.Web.ViewModels.Pages;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPagesService
    {
        T GetById<T>(int id);

        IEnumerable<T> GetForId<T>(int id);

        Task<int> CreateAsync(int profileId, CreatePageInputModel input, string path);

        bool IsOwner(int profileId, int pageId);

        bool IsOwner(string userId, int pageId);

        bool IsFollowing(int profileId, int pageId);

        Task FollowPageAsync(int profileId, int pageId);

        Task DeleteAsync(int pageId);

        Task UpdateAsync(EditPageInputModel input, string path);
    }
}
