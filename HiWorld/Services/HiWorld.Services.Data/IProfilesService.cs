namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using HiWorld.Web.ViewModels.Profiles;

    public interface IProfilesService
    {
        T GetById<T>(int id);

        DisplayProfileViewModel GetByIdForAccessor(int id, string accessorId);

        Task<int> Create(BaseInfoInputModel input);

        Task SendFriendRequest(int profileId, string senderId);

        Task RemoveFriend(int profileId, string senderId);

        Task FollowProfile(int profileId, string senderId);

        Task UpdateAsync(int id, EditProfileInputModel input, string path);
    }
}
