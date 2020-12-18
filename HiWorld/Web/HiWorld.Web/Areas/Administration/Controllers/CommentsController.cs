namespace HiWorld.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Administration;
    using Microsoft.AspNetCore.Mvc;

    public class CommentsController : AdministrationController
    {
        private readonly ICommentsService commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            this.commentsService = commentsService;
        }

        public IActionResult List()
        {
            var viewModel = this.commentsService.GetAllComments<CommentInfoViewModel>();

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.commentsService.DeleteAsync(id);

            return this.RedirectToAction(nameof(this.List));
        }
    }
}
