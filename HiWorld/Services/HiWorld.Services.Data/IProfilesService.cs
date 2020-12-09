namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Profiles;

    public interface IProfilesService
    {
        int GetId(string userId);

        T GetById<T>(int id);

        T GetByUserId<T>(string id);

        Task<int> Create(BaseInfoInputModel input);

        Task SendFriendRequest(int profileId, string senderId);

        Task RemoveFriend(int profileId, string senderId);

        Task DenyFriendship(int id);

        Task AcceptFriendship(int id);

        Task FollowProfile(int profileId, string senderId);

        Task UpdateAsync(string id, EditProfileInputModel input, string path);

        bool IsOwner(string userId, int profileId);

        bool IsFriend(int profileId, string userId);

        bool IsPending(int profileId, string userId);

        bool IsFollowing(int profileId, string userId);

        IEnumerable<T> GetFriendRequests<T>(string userId);
    }
}
