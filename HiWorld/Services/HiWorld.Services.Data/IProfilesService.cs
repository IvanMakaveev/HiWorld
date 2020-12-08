namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Profiles;

    public interface IProfilesService
    {
        T GetById<T>(int id);

        T GetByUserId<T>(string id);

        DisplayProfileViewModel GetByIdForAccessor(int id, string accessorId);

        Task<int> Create(BaseInfoInputModel input);

        Task SendFriendRequest(int profileId, string senderId);

        Task RemoveFriend(int profileId, string senderId);

        Task DenyFriendship(int id);

        Task AcceptFriendship(int id);

        Task FollowProfile(int profileId, string senderId);

        Task UpdateAsync(string id, EditProfileInputModel input, string path);

        bool IsOwner(string userId, int profileId);

        IEnumerable<T> GetFriendRequests<T>(string userId);
    }
}
