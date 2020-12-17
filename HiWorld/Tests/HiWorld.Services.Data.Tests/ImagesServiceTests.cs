namespace HiWorld.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Moq;
    using Xunit;

    public class ImagesServiceTests
    {
        private const string ImageName = "Test.png";
        private const string ImageContentType = "image/jpg";

        private string imageFolder = $"{Directory.GetCurrentDirectory()}\\imgtest";

        [Fact]
        public async Task ImageCreationWorksCorrectly()
        {
            var list = new List<Image>()
            {
                new Image()
                {
                    Id = "first",
                    Extension = "png",
                },
                new Image()
                {
                    Id = "second",
                    Extension = "jpg",
                },
            };
            var mockRepo = new Mock<IRepository<Image>>();
            mockRepo.Setup(x => x.AddAsync(It.IsAny<Image>())).Callback((Image image) => list.Add(image));

            var imageService = new ImagesService(mockRepo.Object);
            using (var stream = File.OpenRead(ImageName))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = ImageContentType,
                };
                await imageService.Create(file, this.imageFolder);
            }

            var directory = new DirectoryInfo(this.imageFolder);
            directory.GetFiles().ToList().ForEach(x => x.Delete());

            Assert.Equal(3, list.Count);
        }
    }
}
