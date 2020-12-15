namespace HiWorld.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Ganss.XSS;
    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly ICommentsService commentsService;

        public CommentsController(
            IProfilesService profilesService,
            ICommentsService commentsService)
        {
            this.profilesService = profilesService;
            this.commentsService = commentsService;
        }

        [HttpPost]
        public async Task Like(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            await this.commentsService.LikeCommentAsync(id, profileId);
        }

        [HttpPost]
        public async Task<ActionResult<PostCommentResponceModel>> AddComment(PostCommentInputModel input)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var profileId = this.profilesService.GetId(userId);

                var sanitizer = new HtmlSanitizer();
                input.Text = sanitizer.Sanitize(input.Text);

                var viewModel = await this.commentsService.AddCommentAsync<PostCommentResponceModel>(profileId, input);
                return viewModel;
            }

            return this.BadRequest();
        }
    }
}
