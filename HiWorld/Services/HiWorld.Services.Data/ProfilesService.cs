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
    }
}
