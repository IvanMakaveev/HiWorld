namespace HiWorld.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;

    public class TagsService : ITagsService
    {
        private readonly IRepository<Tag> tagsRepository;

        public TagsService(IRepository<Tag> tagsRepository)
        {
            this.tagsRepository = tagsRepository;
        }

        public async Task<int> GetId(string name)
        {
            name = name.Trim().ToLower().Replace(" ", "_");
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
    }
}
