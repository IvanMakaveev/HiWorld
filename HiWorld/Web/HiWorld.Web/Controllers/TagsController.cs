using HiWorld.Services.Data;
using HiWorld.Web.ViewModels.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HiWorld.Web.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ITagsService tagsService;
        private readonly IPostsService postsService;
        private readonly ICommentsService commentsService;
        private readonly IProfilesService profilesService;

        public TagsController(
            ITagsService tagsService,
            IPostsService postsService,
            ICommentsService commentsService,
            IProfilesService profilesService)
        {
            this.tagsService = tagsService;
            this.postsService = postsService;
            this.commentsService = commentsService;
            this.profilesService = profilesService;
        }

        public IActionResult ById(int id)
        {
            var viewModel = this.tagsService.SearchByTag<TagSearchViewModel>(id);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.Posts.ForEach(x => x.IsLiked = this.postsService.IsLiked(x.Id, profileId));
            viewModel.Posts.ForEach(x => x.Comments.ForEach(y => y.IsLiked = this.commentsService.IsLiked(y.Id, profileId)));
            viewModel.Posts.ForEach(x => x.Comments.OrderByDescending(x => x.CreatedOn));

            return this.View(viewModel);
        }
    }
}
