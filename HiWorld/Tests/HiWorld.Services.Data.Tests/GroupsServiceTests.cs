using HiWorld.Data;
using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Data.Models.Enums;
using HiWorld.Data.Repositories;
using HiWorld.Services.Data.Tests.FakeModels;
using HiWorld.Services.Mapping;
using HiWorld.Web.ViewModels.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
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
    public class GroupsServiceTests : IDisposable
    {
        private IDeletableEntityRepository<Group> groupsRepository;
        private IRepository<GroupMember> groupMembersRepository;
        private IRepository<ProfileFriend> friendsRepository;
        private IImagesService imagesService;
        private GroupsService groupsService;
        private ApplicationDbContext dbContext;

        public GroupsServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockImageService = new Mock<IImagesService>();
            mockImageService.Setup(x => x.Create(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Returns(Task.Run(() => "test"));
            this.imagesService = mockImageService.Object;

            var connection = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            this.dbContext = new ApplicationDbContext(connection.Options);
            this.dbContext.Database.EnsureCreated();

            this.friendsRepository = new EfRepository<ProfileFriend>(this.dbContext);
            this.groupMembersRepository = new EfRepository<GroupMember>(this.dbContext);
            this.groupsRepository = new EfDeletableEntityRepository<Group>(this.dbContext);
            this.groupsService = new GroupsService(
                this.groupsRepository,
                this.groupMembersRepository,
                this.friendsRepository,
                this.imagesService);
        }

        [Fact]
        public async Task GetProfileGroupsWorksCorrectly()
        {
            await this.SeedData();

            var result = this.groupsService.GetProfileGroups<FakeGroupModel>(1);

            Assert.Single(result);
        }

        [Fact]
        public async Task IsOwnerWorksCorrectly()
        {
            await this.SeedData();

            var result = this.groupsService.IsOwner(1, 1);
            var secondResult = this.groupsService.IsOwner(1, 2);
            var thirdResult = this.groupsService.IsOwner(1, 3);

            Assert.True(result);
            Assert.False(secondResult);
            Assert.False(thirdResult);
        }

        [Fact]
        public async Task HasAdminPermissionsWorksCorrectly()
        {
            await this.SeedData();

            var result = this.groupsService.HasAdminPermissions(1, 1);
            var secondResult = this.groupsService.HasAdminPermissions(1, 2);
            var thirdResult = this.groupsService.HasAdminPermissions(1, 3);

            Assert.True(result);
            Assert.False(secondResult);
            Assert.True(thirdResult);
        }

        [Fact]
        public async Task IsMemberWorksCorrectly()
        {
            await this.SeedData();

            var result = this.groupsService.IsMember(1, 1);
            var secondResult = this.groupsService.IsMember(1, 2);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task CreateAsyncAddsItemCrorreclty()
        {
            var input = new CreateGroupInputModel()
            {
                Description = "test",
                Name = "test",
                Image = null,
            };

            await this.groupsService.CreateAsync(1, input, "test");
            Assert.Equal(1, await this.groupsRepository.All().CountAsync());
        }

        [Fact]
        public async Task CreateAsyncAddsItemCrorrecltyWithImage()
        {
            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);
            var input = new CreateGroupInputModel()
            {
                Description = "test",
                Name = "test",
                Image = imageMock.Object,
            };

            await this.groupsService.CreateAsync(1, input, "test");
            Assert.Equal(1, await this.groupsRepository.All().CountAsync());
        }

        [Fact]
        public async Task ChangeProfileRoleWorksCorrectly()
        {
            await this.SeedData();

            await this.groupsService.ChangeProfileRole(1, 1, GroupRole.Admin);

            Assert.Equal(GroupRole.Admin, (await this.groupMembersRepository.All().FirstOrDefaultAsync()).Role);
        }

        [Fact]
        public async Task ChangeProfileRoleDoesntChangeIfNonExistentIds()
        {
            await this.SeedData();

            await this.groupsService.ChangeProfileRole(1, 2, GroupRole.Admin);

            Assert.Equal(GroupRole.Owner, (await this.groupMembersRepository.All().FirstOrDefaultAsync()).Role);
        }

        [Fact]
        public async Task RemoveMemberWorksCorrectly()
        {
            await this.SeedData();

            await this.groupsService.RemoveMember(1, 1);

            Assert.Equal(1, await this.groupMembersRepository.All().CountAsync());
        }

        [Fact]
        public async Task RemoveMemberDoesntRemoveIfIncorrectIds()
        {
            await this.SeedData();

            await this.groupsService.RemoveMember(1, 2);

            Assert.Equal(2, await this.groupMembersRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeleteGroupWorksCorrectly()
        {
            await this.SeedData();

            await this.groupsService.DeleteGroup(1);

            Assert.Equal(0, await this.groupsRepository.All().CountAsync());
        }

        [Fact]
        public async Task DeleteGroupDoesntRemoveIfIncorrectIds()
        {
            await this.SeedData();

            await this.groupsService.DeleteGroup(2);

            Assert.Equal(1, await this.groupsRepository.All().CountAsync());
        }

        [Fact]
        public async Task GetMembersWorksCorrectly()
        {
            await this.SeedData();

            var result = this.groupsService.GetMembers<FakeGroupMemberModel>(1);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdWorksCorrectly()
        {
            await this.SeedData();

            var result = this.groupsService.GetById<FakeGroupModel>(1);
            Assert.Equal("test", result.Name);
        }

        [Fact]
        public async Task UpdateGroupWorksCorrectly()
        {
            await this.SeedData();
            var input = new EditGroupInputModel
            {
                Description = "test",
                Name = "test",
                Image = null,
                Id = 1,
            };

            await this.groupsService.UpdateAsync(input, "test");
            var result = this.groupsService.GetById<FakeGroupModel>(1);
            Assert.Equal("test", result.Description);
        }

        [Fact]
        public async Task UpdateGroupWorksCorrectlyWithImage()
        {
            var imageMock = new Mock<IFormFile>();
            imageMock.Setup(x => x.Length).Returns(1);

            await this.SeedData();
            var input = new EditGroupInputModel
            {
                Description = "test",
                Name = "test",
                Image = imageMock.Object,
                Id = 1,
            };

            await this.groupsService.UpdateAsync(input, "test");
            var result = this.groupsService.GetById<FakeGroupModel>(1);
            Assert.Equal("test", result.Description);
        }

        [Fact]
        public async Task UpdateGroupDoesntUpdateWithNonExistentId()
        {
            await this.SeedData();
            var input = new EditGroupInputModel
            {
                Description = "test",
                Name = "test",
                Image = null,
                Id = 2,
            };

            await this.groupsService.UpdateAsync(input, "test");
            var result = this.groupsService.GetById<FakeGroupModel>(1);
            Assert.Null(result.Description);
        }

        [Fact]
        public async Task FriendsToInviteWorksCorreclty()
        {
            await this.SeedData();

            var result = this.groupsService.FriendsToInvite(1, 1);
            Assert.Single(result);
        }

        [Fact]
        public async Task AddMemberWorksCorreclty()
        {
            await this.SeedData();

            await this.groupsService.AddMember(2, 1);
            await this.groupsService.AddMember(2, 1);

            Assert.Equal(3, await this.groupMembersRepository.All().CountAsync());
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        private async Task SeedData()
        {
            await this.groupsRepository.AddAsync(new Group()
            {
                Name = "test",
                GroupMembers = new List<GroupMember>() { new GroupMember { MemberId = 1, Role = GroupRole.Owner }, new GroupMember { MemberId = 3, Role = GroupRole.Admin } },
            });
            await this.groupsRepository.SaveChangesAsync();

            await this.friendsRepository.AddAsync(new ProfileFriend()
            {
                ProfileId = 1,
                FriendId = 2,
                IsAccepted = true,
                Friend = new Profile()
                {
                    FirstName = "test",
                    LastName = "test",
                    GroupMembers = new List<GroupMember>(),
                },
            });

            await this.friendsRepository.SaveChangesAsync();
        }
    }
}
