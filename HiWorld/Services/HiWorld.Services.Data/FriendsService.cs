using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using HiWorld.Web.ViewModels.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public class FriendsService : IFriendsService
    {
        private readonly IRepository<ProfileFriend> friendsRepository;

        public FriendsService(IRepository<ProfileFriend> friendsRepository)
        {
            this.friendsRepository = friendsRepository;
        }

        public async Task SendFriendRequestAsync(int profileId, int senderId)
        {
            if (senderId != profileId)
            {
                if (!this.friendsRepository.AllAsNoTracking()
                .Any(x => (x.ProfileId == senderId && x.FriendId == profileId)
                       || (x.ProfileId == profileId && x.FriendId == senderId)))
                {
                    await this.friendsRepository.AddAsync(new ProfileFriend
                    {
                        ProfileId = senderId,
                        FriendId = profileId,
                        IsAccepted = false,
                    });

                    await this.friendsRepository.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveFriendAsync(int profileId, int senderId)
        {
            var friendship = this.friendsRepository.All()
                .FirstOrDefault(x => (x.ProfileId == senderId && x.FriendId == profileId)
                       || (x.ProfileId == profileId && x.FriendId == senderId));

            if (friendship != null)
            {
                this.friendsRepository.Delete(friendship);

                await this.friendsRepository.SaveChangesAsync();
            }
        }

        public async Task DenyFriendshipAsync(int id, int profileId)
        {
            var friendship = this.friendsRepository.All().FirstOrDefault(x => x.Id == id);

            if (friendship != null && friendship.FriendId == profileId)
            {
                this.friendsRepository.Delete(friendship);

                await this.friendsRepository.SaveChangesAsync();
            }
        }

        public async Task AcceptFriendshipAsync(int id, int profileId)
        {
            var friendship = this.friendsRepository.All().FirstOrDefault(x => x.Id == id);

            if (friendship != null && friendship.FriendId == profileId)
            {
                friendship.IsAccepted = true;
                this.friendsRepository.Update(friendship);

                await this.friendsRepository.SaveChangesAsync();
            }
        }

        public IEnumerable<T> GetFriendRequests<T>(int profileId)
        {
            return this.friendsRepository.All().Where(x => x.FriendId == profileId && x.IsAccepted == false).To<T>().ToList();
        }

        public IEnumerable<FriendViewModel> GetFriends(int profileId)
        {
            return this.friendsRepository.All()
                .Where(x => (x.FriendId == profileId || x.ProfileId == profileId) && x.IsAccepted == true)
                .Select(x => new FriendViewModel()
                {
                    CreatedOn = x.CreatedOn,
                    FirstName = profileId == x.ProfileId ? x.Friend.FirstName : x.Profile.FirstName,
                    LastName = profileId == x.ProfileId ? x.Friend.LastName : x.Profile.LastName,
                    FriendId = profileId == x.ProfileId ? x.FriendId : x.ProfileId,
                    ImagePath = profileId == x.ProfileId ?
                        x.Friend.Image != null ? $"{x.Friend.Image.Id}.{x.Friend.Image.Extension}" : null :
                        x.Profile.Image != null ? $"{x.Profile.Image.Id}.{x.Profile.Image.Extension}" : null,
                }).ToList();
        }
    }
}
