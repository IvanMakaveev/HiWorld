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
        private readonly IRepository<ProfileFollower> followersRepository;
        private readonly IPostsService postsService;
        private readonly ICommentsService commentsService;
        private readonly IImagesService imagesService;

        public ProfilesService(
            IDeletableEntityRepository<Profile> profileRepository,
            IRepository<Country> countriesRepository,
            IRepository<ProfileFollower> followersRepository,
            IPostsService postsService,
            ICommentsService commentsService,
            IImagesService imagesService)
        {
            this.profileRepository = profileRepository;
            this.countriesRepository = countriesRepository;
            this.followersRepository = followersRepository;
            this.postsService = postsService;
            this.commentsService = commentsService;
            this.imagesService = imagesService;
        }

        public int GetId(string userId)
        {
            return this.profileRepository.AllAsNoTracking()
                .Where(x => x.User.Id == userId)
                .Select(x => x.Id)
                .FirstOrDefault();
        }

        public async Task<int> CreateAsync(BaseInfoInputModel input)
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

        public bool IsFriend(int profileId, int accessorId)
        {
            return this.profileRepository.AllAsNoTracking().Any(x => x.Id == profileId &&
                (x.FriendsRecieved.Any(y => y.ProfileId == accessorId && y.IsAccepted == true) ||
                 x.FriendsSent.Any(y => y.FriendId == accessorId && y.IsAccepted == true)));
        }

        public bool IsPending(int profileId, int accessorId)
        {
            return this.profileRepository.AllAsNoTracking().Any(x => x.Id == profileId &&
                (x.FriendsRecieved.Any(y => y.ProfileId == accessorId && y.IsAccepted == false) ||
                 x.FriendsSent.Any(y => y.FriendId == accessorId && y.IsAccepted == false)));
        }

        public bool IsFollowing(int profileId, int accessorId)
        {
            return this.followersRepository.All().Any(x => x.ProfileId == profileId && x.FollowerId == accessorId);
        }

        public async Task FollowProfileAsync(int profileId, int senderId)
        {
            if (senderId != profileId)
            {
                var followRelation = this.followersRepository.All().FirstOrDefault(x => x.FollowerId == senderId && x.ProfileId == profileId);
                if (followRelation == null)
                {
                    await this.followersRepository.AddAsync(new ProfileFollower
                    {
                        ProfileId = profileId,
                        FollowerId = senderId,
                    });
                }
                else
                {
                    this.followersRepository.Delete(followRelation);
                }

                await this.followersRepository.SaveChangesAsync();
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
            var profile = this.profileRepository.All().FirstOrDefault(x => x.User.Id == id);

            if (profile != null)
            {
                this.ValidateCountryId(input.CountryId);

                var gender = this.GetGender(input.Gender);

                profile.FirstName = input.FirstName;
                profile.LastName = input.LastName;
                profile.Gender = gender;
                profile.BirthDate = input.BirthDate;
                profile.About = input.About;
                profile.CountryId = input.CountryId;

                if (input.Image != null && input.Image.Length > 0)
                {
                    profile.ImageId = await this.imagesService.Create(input.Image, path);
                }

                this.profileRepository.Update(profile);
                await this.profileRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var profile = this.profileRepository.All().Where(x => x.Id == id).FirstOrDefault();
            if (profile != null)
            {
                await this.postsService.DeleteAllPostsFromProfile(id);
                await this.commentsService.DeleteAllCommentsFromProfile(id);

                this.profileRepository.Delete(profile);
                await this.profileRepository.SaveChangesAsync();
            }
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
