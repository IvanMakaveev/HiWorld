using HiWorld.Services.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiWorld.Web.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly IProfilesService profilesService;

        public ProfilesController(IProfilesService profilesService)
        {
            this.profilesService = profilesService;
        }

        public IActionResult ById(int id)
        {
            var viewModel = this.profilesService.GetById(id);
            viewModel.AccessorId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return this.View(viewModel);
        }
    }
}
