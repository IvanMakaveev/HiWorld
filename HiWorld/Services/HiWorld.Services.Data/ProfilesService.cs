using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Data.Models.Enums;
using HiWorld.Web.ViewModels.Profiles;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public class ProfilesService : IProfilesService
    {
        private readonly IDeletableEntityRepository<Profile> profileRepository;
        private readonly IRepository<Country> countriesRepository;
        private readonly IRepository<ProfileFriend> friendsRepository;
        private readonly IRepository<ProfileFollower> followersRepository;

        public ProfilesService(
            IDeletableEntityRepository<Profile> profileRepository,
            IRepository<Country> countriesRepository,
            IRepository<ProfileFriend> friendsRepository,
            IRepository<ProfileFollower> followersRepository)
        {
            this.profileRepository = profileRepository;
            this.countriesRepository = countriesRepository;
            this.friendsRepository = friendsRepository;
            this.followersRepository = followersRepository;
        }

        public async Task<int> Create(BaseInfoInputModel input)
        {
            if (this.countriesRepository.AllAsNoTracking().FirstOrDefault(x => x.Id == input.CountryId) == null)
            {
                throw new ArgumentException("The selected Country must be valid");
            }

            Enum.TryParse<Gender>(input.SelectedGender, out Gender gender);
            var profile = new Profile()
            {
                BirthDate = input.BirthDate,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Gender = gender,
                CountryId = input.CountryId,
            };

            await this.profileRepository.AddAsync(profile);
            await this.profileRepository.SaveChangesAsync();

            return profile.Id;
        }

        public DisplayProfileViewModel GetById(int id, string accessorId)
        {
            return this.profileRepository.AllAsNoTracking().Where(x => x.Id == id).Select(x => new DisplayProfileViewModel()
            {
                Id = x.Id,
                IsOwner = x.User.Id == accessorId,
                IsFriend = x.FriendsRecieved.Any(x => x.Profile.User.Id == accessorId && x.IsAccepted == true)
                        || x.FriendsSent.Any(x => x.Friend.User.Id == accessorId && x.IsAccepted == true),
                IsPending = x.FriendsRecieved.Any(x => x.Profile.User.Id == accessorId && x.IsAccepted == false)
                        || x.FriendsSent.Any(x => x.Friend.User.Id == accessorId && x.IsAccepted == false),
                IsFollowing = x.Followers.Any(x => x.Follower.User.Id == accessorId),
                FirstName = x.FirstName,
                LastName = x.LastName,
                About = x.About,
                BirthDate = x.BirthDate,
                Country = x.Country.Name,
                FollowersCount = x.Followers.Count,
                FriendsCount = x.FriendsRecieved.Where(x => x.IsAccepted == true).Count() + x.FriendsSent.Where(x => x.IsAccepted == true).Count(),
                Gender = x.Gender.ToString(),
                ImagePath = x.Image == null ? null : x.Image.Id + x.Image.Extension,
                Posts = x.Posts.Select(post => new ProfilePostViewModel()
                {
                    Text = post.Text,
                    PostTags = post.PostTags.Select(tag => tag.Tag.Name).ToList(),
                    ImagePath = post.Image == null ? null : post.Image.Id + x.Image.Extension,
                    CreatedOn = post.CreatedOn,
                }).ToList(),
            }).FirstOrDefault();
        }

        public async Task SendFriendRequest(int profileId, string senderId)
        {
            var senderProfile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == senderId);
            var recieverProfile = this.profileRepository.All().FirstOrDefault(x => x.Id == profileId);

            if (!this.friendsRepository.AllAsNoTracking()
                .Any(x => (x.Profile == senderProfile && x.Friend == recieverProfile)
                       || (x.Profile == recieverProfile && x.Friend == senderProfile)))
            {
                await this.friendsRepository.AddAsync(new ProfileFriend
                {
                    Profile = senderProfile,
                    Friend = recieverProfile,
                    IsAccepted = false,
                });

                await this.friendsRepository.SaveChangesAsync();
            }
        }

        public async Task FollowProfile(int profileId, string senderId)
        {
            var senderProfile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == senderId);
            var recieverProfile = this.profileRepository.All().FirstOrDefault(x => x.Id == profileId);

            var followRelation = this.followersRepository.All().FirstOrDefault(x => x.Follower == senderProfile && x.Profile == recieverProfile);
            if (followRelation == null)
            {
                await this.followersRepository.AddAsync(new ProfileFollower
                {
                    Profile = recieverProfile,
                    Follower = senderProfile,
                });

                await this.followersRepository.SaveChangesAsync();
            }
            else
            {
                this.followersRepository.Delete(followRelation);
                await this.followersRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveFriend(int profileId, string senderId)
        {
            var senderProfile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == senderId);
            var recieverProfile = this.profileRepository.All().FirstOrDefault(x => x.Id == profileId);

            var friendship = this.friendsRepository.All()
                .FirstOrDefault(x => (x.Profile == senderProfile && x.Friend == recieverProfile)
                       || (x.Profile == recieverProfile && x.Friend == senderProfile));

            if (friendship != null)
            {
                this.friendsRepository.Delete(friendship);

                await this.friendsRepository.SaveChangesAsync();
            }
        }
    }
}
