namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using HiWorld.Data.Common.Repositories;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;
    using HiWorld.Web.ViewModels.Posts;

    public class PostsService : IPostsService
    {
        private readonly IDeletableEntityRepository<Post> postsRepository;
        private readonly IRepository<PostTag> postTagsRepository;
        private readonly IRepository<PostLike> postLikesRepository;
        private readonly IImagesService imagesService;
        private readonly ITagsService tagsService;

        public PostsService(
            IDeletableEntityRepository<Post> postsRepository,
            IRepository<PostTag> postTagsRepository,
            IRepository<PostLike> postLikesRepository,
            IImagesService imagesService,
            ITagsService tagsService)
        {
            this.postsRepository = postsRepository;
            this.postTagsRepository = postTagsRepository;
            this.postLikesRepository = postLikesRepository;
            this.imagesService = imagesService;
            this.tagsService = tagsService;
        }

        public async Task CreateForProfileAsync(int id, CreatePostInputModel input, string path)
        {
            var post = new Post()
            {
                ProfileId = id,
                Text = input.Text,
            };

            if (input.Image != null && input.Image.Length > 0)
            {
                post.ImageId = await this.imagesService.Create(input.Image, path);
            }

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();

            if (input.Tags != null && input.Tags.Count() > 0)
            {
                await this.AddTagsToPost(post.Id, input.Tags);
            }
        }

        public async Task CreateForPageAsync(int id, CreatePostInputModel input, string path)
        {
            var post = new Post()
            {
                PageId = id,
                Text = input.Text,
            };

            if (input.Image != null && input.Image.Length > 0)
            {
                post.ImageId = await this.imagesService.Create(input.Image, path);
            }

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();

            if (input.Tags != null && input.Tags.Count() > 0)
            {
                await this.AddTagsToPost(post.Id, input.Tags);
            }
        }

        public async Task DeletePostFromProfileAsync(int profileId, int id)
        {
            var post = this.postsRepository.All().Where(x => x.Id == id && x.ProfileId == profileId).FirstOrDefault();
            if (post != null)
            {
                this.postsRepository.Delete(post);
                await this.postsRepository.SaveChangesAsync();
            }
        }

        public async Task DeletePostFromPageAsync(int pageId, int id)
        {
            var post = this.postsRepository.All().Where(x => x.Id == id && x.PageId == pageId).FirstOrDefault();
            if (post != null)
            {
                this.postsRepository.Delete(post);
                await this.postsRepository.SaveChangesAsync();
            }
        }

        public async Task DeleteAllPostsFromPage(int pageId)
        {
            var posts = this.postsRepository.All().Where(x => x.PageId == pageId).ToList();
            foreach (var post in posts)
            {
                this.postsRepository.Delete(post);
            }

            await this.postsRepository.SaveChangesAsync();
        }

        public async Task LikePostAsync(int profileId, int id)
        {
            var postLike = this.postLikesRepository.All().FirstOrDefault(x => x.PostId == id && x.ProfileId == profileId);
            if (postLike == null)
            {
                postLike = new PostLike()
                {
                    ProfileId = profileId,
                    PostId = id,
                };

                await this.postLikesRepository.AddAsync(postLike);
            }
            else
            {
                this.postLikesRepository.Delete(postLike);
            }

            await this.postLikesRepository.SaveChangesAsync();
        }

        public bool IsLiked(int postId, int accessorId)
        {
            return this.postLikesRepository.AllAsNoTracking().Any(x => x.PostId == postId && x.ProfileId == accessorId);
        }

        public IEnumerable<T> GetProfilePosts<T>(int profileId, int pageNumber, int count = 20)
        {
            return this.postsRepository.AllAsNoTracking()
                .Where(x => x.ProfileId == profileId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .To<T>()
                .ToList();
        }

        public IEnumerable<T> GetPagePosts<T>(int pageId, int pageNumber, int count = 20)
        {
            return this.postsRepository.AllAsNoTracking()
                .Where(x => x.PageId == pageId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .To<T>()
                .ToList();
        }

        public int GetProfileTotalPosts(int profileId)
        {
            return this.postsRepository.AllAsNoTracking()
                .Where(x => x.ProfileId == profileId).Count();
        }

        public int GetPageTotalPosts(int pageId)
        {
            return this.postsRepository.AllAsNoTracking()
                .Where(x => x.PageId == pageId).Count();
        }

        public bool IsOwner(int postId, bool isProfile, int accessorId)
        {
            if (isProfile)
            {
                return this.postsRepository.AllAsNoTracking().Where(x => x.Id == postId).FirstOrDefault()?.ProfileId == accessorId;
            }
            else
            {
                return this.postsRepository.AllAsNoTracking().Where(x => x.Id == postId).FirstOrDefault()?.PageId == accessorId;
            }
        }

        private async Task AddTagsToPost(int postId, IEnumerable<string> tags)
        {
            foreach (var tag in tags.Distinct())
            {
                var tagId = await this.tagsService.GetIdAsync(tag);
                var postTag = new PostTag()
                {
                    TagId = tagId,
                    PostId = postId,
                };

                await this.postTagsRepository.AddAsync(postTag);
                await this.postTagsRepository.SaveChangesAsync();
            }
        }
    }
}
