namespace HiWorld.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Administration;
    using Microsoft.AspNetCore.Mvc;

    public class PagesController : AdministrationController
    {
        private readonly IPagesService pagesService;

        public PagesController(IPagesService pagesService)
        {
            this.pagesService = pagesService;
        }

        public IActionResult List()
        {
            var viewModel = this.pagesService.GetAllPages<PageFullInfoViewModel>();

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.pagesService.DeleteAsync(id);

            return this.RedirectToAction(nameof(this.List));
        }
    }
}
