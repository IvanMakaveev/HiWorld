namespace HiWorld.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Data.Models.Enums;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Groups;

    public class GroupsService : IGroupsService
    {
        private readonly IDeletableEntityRepository<Group> groupsRepository;
        private readonly IRepository<GroupMember> groupMembersRepository;
        private readonly IRepository<ProfileFriend> friendsRepository;
        private readonly IImagesService imagesService;

        public GroupsService(
            IDeletableEntityRepository<Group> groupsRepository,
            IRepository<GroupMember> groupMembersRepository,
            IRepository<ProfileFriend> friendsRepository,
            IImagesService imagesService)
        {
            this.groupsRepository = groupsRepository;
            this.groupMembersRepository = groupMembersRepository;
            this.friendsRepository = friendsRepository;
            this.imagesService = imagesService;
        }

        public IEnumerable<T> GetProfileGroups<T>(int profileId)
        {
            return this.groupsRepository.AllAsNoTracking()
                .Where(x => x.GroupMembers
                    .Any(y => y.MemberId == profileId))
                .To<T>()
                .ToList();
        }

        public bool IsOwner(int groupId, int profileId)
        {
            return this.groupMembersRepository.AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.MemberId == profileId).FirstOrDefault()?.Role == GroupRole.Owner;
        }

        public bool HasAdminPermissions(int groupId, int profileId)
        {
            var role = this.groupMembersRepository.AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.MemberId == profileId).FirstOrDefault()?.Role;

            return role == GroupRole.Admin || role == GroupRole.Owner;
        }

        public bool IsMember(int groupId, int profileId)
        {
            return this.groupMembersRepository.AllAsNoTracking()
                .Where(x => x.GroupId == groupId && x.MemberId == profileId).FirstOrDefault() != null;
        }

        public async Task<int> CreateAsync(int profileId, CreateGroupInputModel input, string path)
        {
            var group = new Group()
            {
                Name = input.Name,
                Description = input.Description,
            };

            if (input.Image != null && input.Image.Length > 0)
            {
                group.ImageId = await this.imagesService.Create(input.Image, path);
            }

            await this.groupsRepository.AddAsync(group);
            await this.groupsRepository.SaveChangesAsync();

            var groupMember = new GroupMember()
            {
                MemberId = profileId,
                GroupId = group.Id,
                Role = GroupRole.Owner,
            };

            await this.groupMembersRepository.AddAsync(groupMember);
            await this.groupMembersRepository.SaveChangesAsync();

            return group.Id;
        }

        public async Task ChangeProfileRole(int profileId, int groupId, GroupRole role)
        {
            var groupMember = this.groupMembersRepository.All()
                .Where(x => x.MemberId == profileId && x.GroupId == groupId).FirstOrDefault();

            if (groupMember != null)
            {
                groupMember.Role = role;

                this.groupMembersRepository.Update(groupMember);
                await this.groupMembersRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveMember(int profileId, int groupId)
        {
            var groupMember = this.groupMembersRepository.All()
                .Where(x => x.MemberId == profileId && x.GroupId == groupId).FirstOrDefault();

            if (groupMember != null)
            {
                this.groupMembersRepository.Delete(groupMember);
                await this.groupMembersRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteGroup(int groupId)
        {
            var group = this.groupsRepository.All()
                .Where(x => x.Id == groupId).FirstOrDefault();

            if (group != null)
            {
                this.groupsRepository.Delete(group);
                await this.groupMembersRepository.SaveChangesAsync();
            }
        }

        public IEnumerable<T> GetMembers<T>(int groupId)
        {
            return this.groupMembersRepository.AllAsNoTracking().Where(x => x.GroupId == groupId).To<T>().ToList();
        }

        public T GetById<T>(int groupId)
        {
            return this.groupsRepository.AllAsNoTracking().Where(x => x.Id == groupId).To<T>().FirstOrDefault();
        }

        public async Task UpdateAsync(EditGroupInputModel input, string path)
        {
            var group = this.groupsRepository.All().Where(x => x.Id == input.Id).FirstOrDefault();

            if (group != null)
            {
                group.Name = input.Name;
                group.Description = input.Description;

                if (input.Image != null && input.Image.Length > 0)
                {
                    group.ImageId = await this.imagesService.Create(input.Image, path);
                }

                this.groupsRepository.Update(group);
                await this.groupsRepository.SaveChangesAsync();
            }
        }

        public IEnumerable<GroupFriendAddViewModel> FriendsToInvite(int groupId, int profileId)
        {
            return this.friendsRepository.All()
                .Where(x => (x.FriendId == profileId || x.ProfileId == profileId) && x.IsAccepted == true)
                .Select(x => new GroupFriendAddViewModel()
                {
                    IsInGroup = profileId == x.ProfileId ?
                        x.Friend.GroupMembers.Any(x => x.GroupId == groupId) :
                        x.Profile.GroupMembers.Any(x => x.GroupId == groupId),
                    FirstName = profileId == x.ProfileId ? x.Friend.FirstName : x.Profile.FirstName,
                    LastName = profileId == x.ProfileId ? x.Friend.LastName : x.Profile.LastName,
                    FriendId = profileId == x.ProfileId ? x.FriendId : x.ProfileId,
                    ImagePath = profileId == x.ProfileId ?
                        x.Friend.Image != null ? $"{x.Friend.Image.Id}.{x.Friend.Image.Extension}" : null :
                        x.Profile.Image != null ? $"{x.Profile.Image.Id}.{x.Profile.Image.Extension}" : null,
                }).ToList();
        }

        public async Task AddMember(int profileId, int groupId)
        {
            var isMember = this.groupMembersRepository
                .AllAsNoTracking()
                .Any(x => x.GroupId == groupId && x.MemberId == profileId);

            if (!isMember)
            {
                var groupMember = new GroupMember()
                {
                    GroupId = groupId,
                    MemberId = profileId,
                    Role = GroupRole.Member,
                };

                await this.groupMembersRepository.AddAsync(groupMember);
                await this.groupMembersRepository.SaveChangesAsync();
            }
        }
    }
}