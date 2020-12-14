using HiWorld.Data.Models.Enums;
using HiWorld.Web.ViewModels.Groups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public interface IGroupsService
    {
        IEnumerable<T> GetProfileGroups<T>(int profileId);

        T GetById<T>(int groupId);

        bool IsOwner(int groupId, int profileId);

        bool HasAdminPermissions(int groupId, int profileId);

        bool IsMember(int groupId, int profileId);

        Task<int> CreateAsync(int profileId, CreateGroupInputModel input, string path);

        Task AddProfileToGroup(int profileId, int groupId, GroupRole role);

        Task ChangeProfileRole(int profileId, int groupId, GroupRole role);

        Task AddMember(int profileId, int groupId);

        Task RemoveMember(int profileId, int groupId);

        Task DeleteGroup(int groupId);

        IEnumerable<T> GetMembers<T>(int groupId);

        Task UpdateAsync(EditGroupInputModel input, string path);

        IEnumerable<GroupFriendAddViewModel> FriendsToInvite(int groupId, int profileId);
    }
}
