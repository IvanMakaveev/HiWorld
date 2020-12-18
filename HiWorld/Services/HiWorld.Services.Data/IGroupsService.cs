namespace HiWorld.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data.Models.Enums;
    using HiWorld.Web.ViewModels.Groups;

    public interface IGroupsService
    {
        IEnumerable<T> GetProfileGroups<T>(int profileId);

        T GetById<T>(int groupId);

        bool IsOwner(int groupId, int profileId);

        bool HasAdminPermissions(int groupId, int profileId);

        bool IsMember(int groupId, int profileId);

        Task<int> CreateAsync(int profileId, CreateGroupInputModel input, string path);

        Task ChangeProfileRoleAsync(int profileId, int groupId, GroupRole role);

        Task AddMemberAsync(int profileId, int groupId);

        Task RemoveMemberAsync(int profileId, int groupId);

        Task DeleteGroupAsync(int groupId);

        IEnumerable<T> GetMembers<T>(int groupId);

        Task UpdateAsync(EditGroupInputModel input, string path);

        IEnumerable<GroupFriendAddViewModel> FriendsToInvite(int groupId, int profileId);
    }
}
