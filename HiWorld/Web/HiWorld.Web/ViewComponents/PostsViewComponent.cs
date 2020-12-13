using HiWorld.Services.Data;
using HiWorld.Web.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiWorld.Web.ViewComponents
{
    public class PostsViewComponent : ViewComponent
    {
        private readonly IPostsService postsService;
        private readonly ICommentsService commentsService;
        private readonly IProfilesService profilesService;

        public PostsViewComponent(
            IPostsService postsService,
            ICommentsService commentsService,
            IProfilesService profilesService)
        {
            this.postsService = postsService;
            this.commentsService = commentsService;
            this.profilesService = profilesService;
        }

        public IViewComponentResult Invoke(IEnumerable<PostViewModel> model)
        {
            var userId = this.UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);

            var viewModel = model.ToList();

            viewModel.ForEach(x => x.IsOwner = this.postsService.IsOwner(x.Id, x.IsProfilePost, profileId));
            viewModel.ForEach(x => x.IsLiked = this.postsService.IsLiked(x.Id, profileId));
            viewModel.ForEach(x => x.Comments.ForEach(y => y.IsLiked = this.commentsService.IsLiked(y.Id, profileId)));
            viewModel.ForEach(x => x.Comments.OrderByDescending(x => x.CreatedOn));

            return this.View(viewModel);
        }
    }
}
