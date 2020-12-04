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

        public ProfilesService(
            IDeletableEntityRepository<Profile> profileRepository,
            IRepository<Country> countriesRepository)
        {
            this.profileRepository = profileRepository;
            this.countriesRepository = countriesRepository;
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

        public DisplayProfileViewModel GetById(int id)
        {
            return this.profileRepository.AllAsNoTracking().Where(x => x.Id == id).Select(x => new DisplayProfileViewModel()
            {
                OwnerUserId = x.User.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                About = x.About,
                BirthDate = x.BirthDate,
                Country = x.Country.Name,
                FollowersCount = x.Followers.Count,
                FriendsCount = x.FriendsRecieved.Count + x.FriendsSent.Count,
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
    }
}
