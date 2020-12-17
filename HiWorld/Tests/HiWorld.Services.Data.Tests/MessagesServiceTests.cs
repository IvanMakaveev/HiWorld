using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Services.Data.Tests.FakeModels;
using HiWorld.Services.Mapping;
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
    public class MessagesServiceTests
    {
        private List<Message> repoStorage = new List<Message>();
        private IDeletableEntityRepository<Message> messagesRepo;
        private MessagesService messagesService;

        public MessagesServiceTests()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("HiWorld.Services.Data.Tests"));

            var mockRepo = new Mock<IDeletableEntityRepository<Message>>();
            mockRepo.Setup(x => x.AddAsync(It.IsAny<Message>()))
                .Callback((Message msg) => this.repoStorage.Add(msg));
            mockRepo.Setup(x => x.AllAsNoTracking())
                .Returns(this.repoStorage.AsQueryable);
            mockRepo.Setup(x => x.All())
                .Returns(this.repoStorage.AsQueryable);
            mockRepo.Setup(x => x.Delete(It.IsAny<Message>()))
                .Callback((Message msg) => this.repoStorage.Remove(msg));

            this.messagesRepo = mockRepo.Object;
            this.messagesService = new MessagesService(this.messagesRepo);
        }

        [Fact]
        public async Task AddMessageWorksCorrectly()
        {
            var messageText = "test";

            await this.messagesService.AddMessage(1, 1, messageText);

            Assert.Single(this.repoStorage);
            Assert.Equal(messageText, this.repoStorage[0].Text);
        }

        [Fact]
        public async Task DeleteMessageWorksCorrectly()
        {
            this.repoStorage.Add(new Message() { Id = 1 });

            await this.messagesService.DeleteMessage(1);

            Assert.Empty(this.repoStorage);
        }

        [Fact]
        public async Task DeleteMessageDoesntRemoveWhenGivenNonExistentMessageId()
        {
            this.repoStorage.Add(new Message() { Id = 2 });

            await this.messagesService.DeleteMessage(1);

            Assert.Single(this.repoStorage);
        }

        [Fact]
        public void GetMessagesWorksCorrectly()
        {
            var messageText = "test";

            this.repoStorage.Add(new Message
            {
                Id = 1,
                Text = messageText,
            });

            var result = this.messagesService.GetById<FakeMessageModel>(1);

            Assert.Equal(messageText, result.Text);
        }

        [Fact]
        public void GetMessagesReturnsNullWhenMissing()
        {
            var messageText = "test";

            this.repoStorage.Add(new Message
            {
                Id = 1,
                Text = messageText,
            });

            var result = this.messagesService.GetById<FakeMessageModel>(2);

            Assert.Null(result);
        }

        [Fact]
        public void IsOwnerWorksCorrectly()
        {
            this.repoStorage.Add(new Message
            {
                Id = 1,
                ProfileId = 1,
            });

            var firstResult = this.messagesService.IsOwner(1, 1);
            var secondResult = this.messagesService.IsOwner(1, 2);

            Assert.True(firstResult);
            Assert.False(secondResult);
        }
    }
}
