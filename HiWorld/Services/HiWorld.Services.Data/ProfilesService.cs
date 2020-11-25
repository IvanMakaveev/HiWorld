using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Data.Models.Enums;
using HiWorld.Web.ViewModels.Profiles;
using System;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public class ProfilesService : IProfilesService
    {
        private readonly IDeletableEntityRepository<Profile> profileRepository;

        public ProfilesService(IDeletableEntityRepository<Profile> profileRepository)
        {
            this.profileRepository = profileRepository;
        }

        public async Task<int> Create(BaseInfoInputModel input)
        {
            Enum.TryParse<Gender>(input.SelectedGender, out Gender gender);
            var profile = new Profile()
            {
                BirthDate = input.BirthDate,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Gender = gender,
                CityId = 1,
            };

            await this.profileRepository.AddAsync(profile);
            await this.profileRepository.SaveChangesAsync();

            return profile.Id;
        }
    }
}
