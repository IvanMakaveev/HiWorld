namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public IEnumerable<KeyValuePair<int, string>> GetAllAsKvp()
        {
            return this.countriesRepository
                .AllAsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                })
                .OrderBy(x => x.Name)
                .ToList()
                .Select(x => new KeyValuePair<int, string>(x.Id, x.Name));
        }
    }
}
