namespace HiWorld.Services.Data
{
    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Pages;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PagesService : IPagesService
    {
        private readonly IDeletableEntityRepository<Page> pagesRepository;
        private readonly IRepository<PageFollower> followersRepository;
        private readonly IImagesService imagesService;

        public PagesService(
            IDeletableEntityRepository<Page> pagesRepository,
            IRepository<PageFollower> followersRepository,
            IImagesService imagesService)
        {
            this.pagesRepository = pagesRepository;
            this.followersRepository = followersRepository;
            this.imagesService = imagesService;
        }

        public async Task<int> Create(int profileId, CreatePageInputModel input, string path)
        {
            var page = new Page()
            {
                Name = input.Name,
                Description = input.Description,
                Email = input.Email,
                Phone = input.Phone,
                ProfileId = profileId,
            };

            if (input.Image != null && input.Image.Length > 0)
            {
                page.ImageId = await this.imagesService.Create(input.Image, path);
            }

            await this.pagesRepository.AddAsync(page);
            await this.pagesRepository.SaveChangesAsync();

            return page.Id;
        }

        public T GetById<T>(int id)
        {
            return this.pagesRepository.AllAsNoTracking().Where(x => x.ProfileId == id).To<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetForId<T>(int id)
        {
            return this.pagesRepository.AllAsNoTracking().Where(x => x.ProfileId == id).To<T>().ToList();
        }

        public bool IsFollowing(int profileId, int pageId)
        {
            return this.followersRepository.AllAsNoTracking().Any(x => x.PageId == profileId && x.FollowerId == profileId);
        }

        public bool IsOwner(int profileId, int pageId)
        {
            return this.pagesRepository.AllAsNoTracking().Any(x => x.ProfileId == profileId && x.Id == pageId);
        }
    }
}
