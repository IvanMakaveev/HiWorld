namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class TagsService : ITagsService
    {
        private readonly IRepository<Tag> tagsRepository;

        public TagsService(IRepository<Tag> tagsRepository)
        {
            this.tagsRepository = tagsRepository;
        }

        public async Task<int> GetIdAsync(string name)
        {
            name = name.Trim().ToLower().Replace(" ", string.Empty);
            var tag = this.tagsRepository.All().FirstOrDefault(x => x.Name == name);
            if (tag == null)
            {
                tag = new Tag()
                {
                    Name = name,
                };

                await this.tagsRepository.AddAsync(tag);
                await this.tagsRepository.SaveChangesAsync();
            }

            return tag.Id;
        }

        public string GetName(int id)
        {
            return this.tagsRepository.AllAsNoTracking().Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
        }

        public IEnumerable<T> SearchPagesByTag<T>(int id, int pageNumber, int count = 20)
        {
            return this.tagsRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.PageTags.Select(x => x.Page))
                .OrderByDescending(x => x.PageFollowers.Count)
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .To<T>()
                .ToList();
        }

        public int SearchPagesByTagCount(int id)
        {
            return this.tagsRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.PageTags.Select(x => x.Page)).Count();
        }

        public IEnumerable<T> SearchPostsByTag<T>(int id, int pageNumber, int count = 20)
        {
            return this.tagsRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.PostTags.Select(x => x.Post))
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .To<T>()
                .ToList();
        }

        public int SearchPostsByTagCount(int id)
        {
            return this.tagsRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .SelectMany(x => x.PostTags.Select(x => x.Post)).Count();
        }
    }
}
