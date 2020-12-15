using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace HiWorld.Services.Data.Tests
{
    public class CountriesServiceTests
    {
        [Fact]
        public void CheckIfGettingAllAsKvpWorksCorrectly()
        {
            var list = new List<Country>()
            {
                new Country()
                {
                    Id = 1,
                    Name = "Bulgaria",
                },
                new Country()
                {
                    Id = 2,
                    Name = "Germany",
                },
            };
            var mockRepo = new Mock<IRepository<Country>>();
            mockRepo.Setup(x => x.AllAsNoTracking()).Returns(list.AsQueryable());
            var service = new CountriesService(mockRepo.Object);

            var actualResult = service.GetAllAsKvp();

            Assert.Equal(2, actualResult.Count());
            Assert.Equal("Bulgaria", actualResult.First().Value);
        }
    }
}
