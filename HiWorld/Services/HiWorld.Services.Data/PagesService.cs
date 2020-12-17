namespace HiWorld.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Pages;

    public class PagesService : IPagesService
    {
        private readonly IDeletableEntityRepository<Page> pagesRepository;
        private readonly IRepository<PageFollower> followersRepository;
        private readonly IRepository<PageTag> pageTagsRepository;
        private readonly ITagsService tagsService;
        private readonly IImagesService imagesService;
        private readonly IPostsService postsService;

        public PagesService(
            IDeletableEntityRepository<Page> pagesRepository,
            IRepository<PageFollower> followersRepository,
            IRepository<PageTag> pageTagsRepository,
            ITagsService tagsService,
            IImagesService imagesService,
            IPostsService postsService)
        {
            this.pagesRepository = pagesRepository;
            this.postsService = postsService;
            this.followersRepository = followersRepository;
            this.tagsService = tagsService;
            this.pageTagsRepository = pageTagsRepository;
            this.imagesService = imagesService;
        }

        public async Task<int> CreateAsync(int profileId, CreatePageInputModel input, string path)
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

            if (input.Tags != null && input.Tags.Count() > 0)
            {
                foreach (var tag in input.Tags.Distinct())
                {
                    var tagId = await this.tagsService.GetIdAsync(tag);
                    var pageTag = new PageTag()
                    {
                        TagId = tagId,
                        PageId = page.Id,
                    };

                    await this.pageTagsRepository.AddAsync(pageTag);
                }

                await this.pageTagsRepository.SaveChangesAsync();
            }

            return page.Id;
        }

        public T GetById<T>(int id)
        {
            return this.pagesRepository.AllAsNoTracking().Where(x => x.Id == id).To<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetProfilePages<T>(int profileId)
        {
            return this.pagesRepository.AllAsNoTracking().Where(x => x.ProfileId == profileId).To<T>().ToList();
        }

        public bool IsFollowing(int profileId, int pageId)
        {
            return this.followersRepository.AllAsNoTracking().Any(x => x.PageId == pageId && x.FollowerId == profileId);
        }

        public bool IsOwner(int profileId, int pageId)
        {
            return this.pagesRepository.AllAsNoTracking().Any(x => x.ProfileId == profileId && x.Id == pageId);
        }

        public bool IsOwner(string userId, int pageId)
        {
            return this.pagesRepository.AllAsNoTracking().Any(x => x.Profile.User.Id == userId && x.Id == pageId);
        }

        public async Task FollowPageAsync(int profileId, int pageId)
        {
            if (!this.IsOwner(profileId, pageId))
            {
                var followRelation = this.followersRepository.All().FirstOrDefault(x => x.FollowerId == profileId && x.PageId == pageId);
                if (followRelation == null)
                {
                    await this.followersRepository.AddAsync(new PageFollower
                    {
                        PageId = pageId,
                        FollowerId = profileId,
                    });
                }
                else
                {
                    this.followersRepository.Delete(followRelation);
                }

                await this.followersRepository.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(EditPageInputModel input, string path)
        {
            var page = this.pagesRepository.All().Where(x => x.Id == input.Id).FirstOrDefault();

            if (page != null)
            {
                page.Name = input.Name;
                page.Description = input.Description;
                page.Email = input.Email;
                page.Phone = input.Phone;

                var tagIds = new List<int>();
                if (input.Tags != null && input.Tags.Count() > 0)
                {
                    foreach (var tag in input.Tags.Distinct())
                    {
                        var tagId = await this.tagsService.GetIdAsync(tag);
                        if (!this.pageTagsRepository.AllAsNoTracking().Any(x => x.TagId == tagId && x.PageId == page.Id))
                        {
                            await this.pageTagsRepository.AddAsync(new PageTag()
                            {
                                TagId = tagId,
                                PageId = page.Id,
                            });
                        }
                    }
                }

                var tagsToRemove = this.pageTagsRepository.All().Where(x => x.PageId == page.Id && !tagIds.Contains(x.TagId)).ToList();
                foreach (var tag in tagsToRemove)
                {
                    this.pageTagsRepository.Delete(tag);
                }

                await this.pageTagsRepository.SaveChangesAsync();

                if (input.Image != null && input.Image.Length > 0)
                {
                    page.ImageId = await this.imagesService.Create(input.Image, path);
                }

                this.pagesRepository.Update(page);
                await this.pagesRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int pageId)
        {
            var page = this.pagesRepository.All().Where(x => x.Id == pageId).FirstOrDefault();

            if (page != null)
            {
                await this.postsService.DeleteAllPostsFromPage(pageId);

                this.pagesRepository.Delete(page);
                await this.pagesRepository.SaveChangesAsync();
            }
        }
    }
}
