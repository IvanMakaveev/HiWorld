namespace HiWorld.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using HiWorld.Data.Models;

    internal class CountriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Countries.Any())
            {
                return;
            }

            await dbContext.Countries.AddAsync(new Country { Name = "United States of America" });
            await dbContext.Countries.AddAsync(new Country { Name = "United Kingdom" });
            await dbContext.Countries.AddAsync(new Country { Name = "Germany" });
            await dbContext.Countries.AddAsync(new Country { Name = "Bulgaria" });
            await dbContext.Countries.AddAsync(new Country { Name = "Russia" });
            await dbContext.Countries.AddAsync(new Country { Name = "Canada" });
            await dbContext.Countries.AddAsync(new Country { Name = "Romania" });
            await dbContext.Countries.AddAsync(new Country { Name = "Serbia" });
            await dbContext.Countries.AddAsync(new Country { Name = "Turkey" });
            await dbContext.Countries.AddAsync(new Country { Name = "France" });
            await dbContext.Countries.AddAsync(new Country { Name = "Spain" });
            await dbContext.Countries.AddAsync(new Country { Name = "Portugal" });
            await dbContext.Countries.AddAsync(new Country { Name = "Brazil" });
            await dbContext.Countries.AddAsync(new Country { Name = "China" });
            await dbContext.Countries.AddAsync(new Country { Name = "Japan" });
            await dbContext.Countries.AddAsync(new Country { Name = "India" });
            await dbContext.Countries.AddAsync(new Country { Name = "Australia" });
        }
    }
}
