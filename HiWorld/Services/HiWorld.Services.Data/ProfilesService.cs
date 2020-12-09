namespace HiWorld.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Data.Models.Enums;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Profiles;
    using Microsoft.EntityFrameworkCore;

    public class ProfilesService : IProfilesService
    {
        private readonly IDeletableEntityRepository<Profile> profileRepository;
        private readonly IRepository<Country> countriesRepository;
        private readonly IRepository<ProfileFriend> friendsRepository;
        private readonly IRepository<ProfileFollower> followersRepository;
        private readonly IRepository<Image> imageRepository;

        public ProfilesService(
            IDeletableEntityRepository<Profile> profileRepository,
            IRepository<Country> countriesRepository,
            IRepository<ProfileFriend> friendsRepository,
            IRepository<ProfileFollower> followersRepository,
            IRepository<Image> imageRepository)
        {
            this.profileRepository = profileRepository;
            this.countriesRepository = countriesRepository;
            this.friendsRepository = friendsRepository;
            this.followersRepository = followersRepository;
            this.imageRepository = imageRepository;
        }

        public int GetId(string userId)
        {
            return this.profileRepository.AllAsNoTracking()
                .Where(x => x.User.Id == userId)
                .Select(x => x.Id)
                .FirstOrDefault();
        }

        public async Task<int> Create(BaseInfoInputModel input)
        {
            this.ValidateCountryId(input.CountryId);

            var gender = this.GetGender(input.Gender);

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

        public bool IsFriend(int profileId, string userId)
        {
            return this.profileRepository.AllAsNoTracking().Any(x => x.Id == profileId &&
                (x.FriendsRecieved.Any(y => y.Profile.User.Id == userId && y.IsAccepted == true) ||
                 x.FriendsSent.Any(y => y.Friend.User.Id == userId && y.IsAccepted == true)));
        }

        public bool IsPending(int profileId, string userId)
        {
            return this.profileRepository.AllAsNoTracking().Any(x => x.Id == profileId &&
                (x.FriendsRecieved.Any(y => y.Profile.User.Id == userId && y.IsAccepted == false) ||
                 x.FriendsSent.Any(y => y.Friend.User.Id == userId && y.IsAccepted == false)));
        }

        public bool IsFollowing(int profileId, string userId)
        {
            return this.followersRepository.All().Any(x => x.ProfileId == profileId && x.Follower.User.Id == userId);
        }

        public async Task SendFriendRequest(int profileId, string senderId)
        {
            var senderProfile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == senderId);
            var recieverProfile = this.profileRepository.All().FirstOrDefault(x => x.Id == profileId);

            if (senderProfile.Id != recieverProfile.Id)
            {
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
        }

        public async Task FollowProfile(int profileId, string senderId)
        {
            var senderProfile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == senderId);
            var recieverProfile = this.profileRepository.All().FirstOrDefault(x => x.Id == profileId);

            if (senderProfile.Id != recieverProfile.Id)
            {
                var followRelation = this.followersRepository.All().FirstOrDefault(x => x.Follower == senderProfile && x.Profile == recieverProfile);
                if (followRelation == null)
                {
                    await this.followersRepository.AddAsync(new ProfileFollower
                    {
                        Profile = recieverProfile,
                        Follower = senderProfile,
                    });
                }
                else
                {
                    this.followersRepository.Delete(followRelation);
                }

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

        public async Task DenyFriendship(int id)
        {
            var friendship = this.friendsRepository.All().FirstOrDefault(x => x.Id == id);

            if (friendship != null)
            {
                this.friendsRepository.Delete(friendship);

                await this.friendsRepository.SaveChangesAsync();
            }
        }

        public async Task AcceptFriendship(int id)
        {
            var friendship = this.friendsRepository.All().FirstOrDefault(x => x.Id == id);

            if (friendship != null)
            {
                friendship.IsAccepted = true;
                this.friendsRepository.Update(friendship);

                await this.friendsRepository.SaveChangesAsync();
            }
        }

        public T GetById<T>(int id)
        {
            return this.profileRepository.All().Where(x => x.Id == id).To<T>().FirstOrDefault();
        }

        public T GetByUserId<T>(string id)
        {
            return this.profileRepository.All().Where(x => x.User.Id == id).To<T>().FirstOrDefault();
        }

        public async Task UpdateAsync(string id, EditProfileInputModel input, string path)
        {
            this.ValidateCountryId(input.CountryId);

            var gender = this.GetGender(input.Gender);

            var profile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == id);
            profile.FirstName = input.FirstName;
            profile.LastName = input.LastName;
            profile.Gender = gender;
            profile.BirthDate = input.BirthDate;
            profile.About = input.About;
            profile.CountryId = input.CountryId;

            if (input.Image != null && input.Image.Length > 0)
            {
                Directory.CreateDirectory($"{path}/");
                var extension = Path.GetExtension(input.Image.FileName).TrimStart('.');

                var image = new Image
                {
                    Extension = extension,
                };
                await this.imageRepository.AddAsync(image);
                await this.imageRepository.SaveChangesAsync();

                var physicalPath = $"{path}/{image.Id}.{extension}";
                using (Stream fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await input.Image.CopyToAsync(fileStream);
                }

                profile.ImageId = image.Id;
            }

            this.profileRepository.Update(profile);
            await this.profileRepository.SaveChangesAsync();
        }

        public bool IsOwner(string userId, int profileId)
        {
            return this.profileRepository.AllAsNoTracking().FirstOrDefault(x => x.Id == profileId && x.User.Id == userId) != null;
        }

        public IEnumerable<T> GetFriendRequests<T>(string userId)
        {
            return this.friendsRepository.All().Where(x => x.Friend.User.Id == userId && x.IsAccepted == false).To<T>().ToList();
        }

        private Gender GetGender(string genderValue)
        {
            Enum.TryParse<Gender>(genderValue, out Gender gender);
            if (gender == 0)
            {
                throw new ArgumentException("The selected Gender must be valid");
            }

            return gender;
        }

        private void ValidateCountryId(int id)
        {
            if (this.countriesRepository.AllAsNoTracking().FirstOrDefault(x => x.Id == id) == null)
            {
                throw new ArgumentException("The selected Country must be valid");
            }
        }
    }
}
