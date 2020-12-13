using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using HiWorld.Services.Mapping;
using HiWorld.Web.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiWorld.Services.Data
{
    public class BrowseService : IBrowseService
    {
        private readonly IDeletableEntityRepository<Profile> profileRepository;
        private readonly IRepository<ProfileFollower> profileFollowersRepository;
        private readonly IRepository<PageFollower> pageFollowersRepository;

        public BrowseService(
            IDeletableEntityRepository<Profile> profileRepository,
            IRepository<ProfileFollower> profileFollowersRepository,
            IRepository<PageFollower> pageFollowersRepository)
        {
            this.profileRepository = profileRepository;
            this.profileFollowersRepository = profileFollowersRepository;
            this.pageFollowersRepository = pageFollowersRepository;
        }

        public IEnumerable<ProfileFollowingViewModel> GetFollowing(int profileId)
        {
            var pages = this.pageFollowersRepository.AllAsNoTracking()
                .Where(x => x.FollowerId == profileId)
                .Select(x => new ProfileFollowingViewModel()
                {
                    Id = x.PageId,
                    Name = x.Page.Name,
                    IsProfile = false,
                    ImagePath = x.Page.Image == null ? null : $"{x.Page.Image.Id}.{x.Page.Image.Extension}",
                })
                .ToList();

            var profiles = this.profileFollowersRepository.AllAsNoTracking()
                .Where(x => x.FollowerId == profileId)
                .Select(x => new ProfileFollowingViewModel()
                {
                    Id = x.ProfileId,
                    Name = $"{x.Profile.FirstName} {x.Profile.LastName}",
                    IsProfile = true,
                    ImagePath = x.Profile.Image == null ? null : $"{x.Profile.Image.Id}.{x.Profile.Image.Extension}",
                })
                .ToList();

            return pages.Concat(profiles);
        }

        public IEnumerable<T> GetNewestPosts<T>(int profileId, int pageNumber, int count = 20)
        {
            var postsFromPages = this.profileRepository.AllAsNoTracking()
                .Where(x => x.Id == profileId)
                .SelectMany(x => x.PageFollows
                    .SelectMany(x => x.Page.Posts))
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .To<T>()
                .ToList();

            var postsFromProfiles = this.profileRepository.AllAsNoTracking()
                .Where(x => x.Id == profileId)
                .SelectMany(x => x.Following
                    .SelectMany(x => x.Profile.Posts))
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * count)
                .Take(count)
                .To<T>()
                .ToList();

            return postsFromPages.Concat(postsFromProfiles);
        }

        public int GetPostsCount(int profileId)
        {
            var postsFromPages = this.profileRepository.AllAsNoTracking()
                .Where(x => x.Id == profileId)
                .SelectMany(x => x.PageFollows
                    .SelectMany(x => x.Page.Posts)).Count();

            var postsFromProfiles = this.profileRepository.AllAsNoTracking()
                .Where(x => x.Id == profileId)
                .SelectMany(x => x.Following
                    .SelectMany(x => x.Profile.Posts)).Count();

            return postsFromPages + postsFromProfiles;
        }
    }
}
