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

        Task<int> CreateAsync(BaseInfoInputModel input);

        Task SendFriendRequestAsync(int profileId, int senderId);

        Task RemoveFriendAsync(int profileId, int senderId);

        Task DenyFriendshipAsync(int id);

        Task AcceptFriendshipAsync(int id);

        Task FollowProfileAsync(int profileId, int senderId);

        Task UpdateAsync(string id, EditProfileInputModel input, string path);

        bool IsFriend(int profileId, int accessorId);

        bool IsPending(int profileId, int accessorId);

        bool IsFollowing(int profileId, int accessorId);

        IEnumerable<T> GetFriendRequests<T>(string userId);
    }
}
