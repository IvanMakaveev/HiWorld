using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Services.Data.Tests.FakeModels;
using HiWorld.Services.Mapping;
using HiWorld.Web.ViewModels;
using HiWorld.Web.ViewModels.Friends;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HiWorld.Services.Data.Tests
{
    public class FriendsServiceTests
    {
        private List<ProfileFriend> repoStorage = new List<ProfileFriend>();
        private IRepository<ProfileFriend> friendsRepo;
        private FriendsService friendsService;

        public FriendsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.GetExecutingAssembly());

            var mockRepo = new Mock<IRepository<ProfileFriend>>();
            mockRepo.Setup(x => x.AddAsync(It.IsAny<ProfileFriend>()))
                .Callback((ProfileFriend friend) => this.repoStorage.Add(friend));
            mockRepo.Setup(x => x.AllAsNoTracking())
                .Returns(this.repoStorage.AsQueryable);
            mockRepo.Setup(x => x.All())
                .Returns(this.repoStorage.AsQueryable);
            mockRepo.Setup(x => x.Delete(It.IsAny<ProfileFriend>()))
                .Callback((ProfileFriend friend) => this.repoStorage.Remove(friend));

            this.friendsRepo = mockRepo.Object;
            this.friendsService = new FriendsService(this.friendsRepo);
        }

        [Fact]
        public async Task CantSendFriendRequestToYourself()
        {
            await this.friendsService.SendFriendRequestAsync(1, 1);

            Assert.Empty(this.repoStorage);
        }

        [Fact]
        public async Task SendFriendRequestAddsCorrectly()
        {
            await this.friendsService.SendFriendRequestAsync(1, 2);

            Assert.Single(this.repoStorage);
        }

        [Fact]
        public async Task CantSendFriendRequestTwice()
        {
            await this.friendsService.SendFriendRequestAsync(2, 1);
            await this.friendsService.SendFriendRequestAsync(1, 2);

            Assert.Single(this.repoStorage);
        }

        [Fact]
        public async Task CantRemoveNonExistentFriend()
        {
            await this.friendsService.SendFriendRequestAsync(1, 2);
            await this.friendsService.RemoveFriendAsync(1, 3);

            Assert.Single(this.repoStorage);
        }

        [Fact]
        public async Task RemoveFriendWorksCorrectly()
        {
            await this.friendsService.SendFriendRequestAsync(1, 2);
            await this.friendsService.RemoveFriendAsync(1, 2);

            Assert.Empty(this.repoStorage);
        }

        [Fact]
        public async Task DenyFriendCannotRemoveNonExistentRequest()
        {
            this.repoStorage.Add(new ProfileFriend { Id = 2, FriendId = 1 });
            await this.friendsService.DenyFriendshipAsync(1, 1);

            Assert.Single(this.repoStorage);
        }

        [Fact]
        public async Task DenyFriendWorksCorrectly()
        {
            this.repoStorage.Add(new ProfileFriend { Id = 1, FriendId = 1 });
            await this.friendsService.DenyFriendshipAsync(1, 1);

            Assert.Empty(this.repoStorage);
        }

        [Fact]
        public async Task AcceptFriendCannotAcceptNonExistentRequest()
        {
            this.repoStorage.Add(new ProfileFriend { Id = 2, FriendId = 1, IsAccepted = false });
            await this.friendsService.AcceptFriendshipAsync(1, 1);

            Assert.False(this.repoStorage[0].IsAccepted);
        }

        [Fact]
        public async Task AcceptFriendWorksCorrectly()
        {
            this.repoStorage.Add(new ProfileFriend { Id = 1, FriendId = 1, IsAccepted = false });
            await this.friendsService.AcceptFriendshipAsync(1, 1);

            Assert.True(this.repoStorage[0].IsAccepted);
        }

        [Fact]
        public void GetFriendRequestsWorksCorrectly()
        {
            this.repoStorage.Add(new ProfileFriend
            {
                Id = 1,
                FriendId = 1,
                IsAccepted = false,
            });

            var result = this.friendsService.GetFriendRequests<FakeFriendRequestModel>(1);

            Assert.Single(result);
        }

        [Fact]
        public void GetFriendsWorksCorrectly()
        {
            this.repoStorage.Add(new ProfileFriend
            {
                Id = 1,
                FriendId = 1,
                IsAccepted = true,
                Profile = new Profile()
                {
                    FirstName = "test",
                    LastName = "test",
                    Image = null,
                },
                CreatedOn = DateTime.Now,
            });

            var result = this.friendsService.GetFriends(1);

            Assert.Single(result);
        }
    }
}
