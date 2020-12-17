namespace HiWorld.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HiWorld.Data.Models.Enums;
    using HiWorld.Services.Data;
    using HiWorld.Web.ViewModels.Groups;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IGroupsService groupsService;
        private readonly IProfilesService profilesService;
        private readonly IWebHostEnvironment webHost;

        public GroupsController(
            IGroupsService groupsService,
            IProfilesService profilesService,
            IWebHostEnvironment webHost)
        {
            this.groupsService = groupsService;
            this.profilesService = profilesService;
            this.webHost = webHost;
        }

        public IActionResult List()
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var viewModel = this.groupsService.GetProfileGroups<GroupInfoViewModel>(profileId).ToList();
            viewModel.ForEach(x => x.IsOwner = this.groupsService.IsOwner(x.Id, profileId));
            viewModel.ForEach(x => x.IsAdmin = this.groupsService.HasAdminPermissions(x.Id, profileId));

            return this.View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new CreateGroupInputModel();

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);

            var groupId = await this.groupsService.CreateAsync(profileId, input, $"{this.webHost.WebRootPath}/img/groups");

            return this.RedirectToAction(nameof(this.ById), new { id = groupId });
        }

        public IActionResult ById(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);
            var isMember = this.groupsService.IsMember(id, profileId);

            if (isMember)
            {
                var viewModel = this.groupsService.GetById<GroupViewModel>(id);
                viewModel.IsAdmin = this.groupsService.HasAdminPermissions(id, profileId);
                viewModel.ProfileId = profileId;

                return this.View(viewModel);
            }

            return this.BadRequest();
        }

        public IActionResult GroupMembers(int id)
        {
            var userid = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userid);
            var hasAdminPermissions = this.groupsService.HasAdminPermissions(id, profileId);

            if (hasAdminPermissions)
            {
                var viewModel = new GroupMembersViewModel()
                {
                    Members = this.groupsService.GetMembers<MemberInfoViewModel>(id),
                    IsAdmin = this.groupsService.HasAdminPermissions(id, profileId),
                    IsOwner = this.groupsService.IsOwner(id, profileId),
                    Id = id,
                };

                return this.View(viewModel);
            }

            return this.BadRequest();
        }

        public IActionResult Edit(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            var hasAdminPermissions = this.groupsService.HasAdminPermissions(id, profileId);

            if (hasAdminPermissions)
            {
                var inputModel = this.groupsService.GetById<EditGroupInputModel>(id);

                return this.View(inputModel);
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditGroupInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            var hasAdminPermissions = this.groupsService.HasAdminPermissions(input.Id, profileId);

            if (hasAdminPermissions)
            {
                await this.groupsService.UpdateAsync(input, $"{this.webHost.WebRootPath}/img/groups");
            }

            return this.RedirectToAction(nameof(this.List));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            var isOwner = this.groupsService.IsOwner(id, profileId);

            if (isOwner)
            {
                await this.groupsService.DeleteGroup(id);
            }

            return this.RedirectToAction(nameof(this.List));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(int id, int profileId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessorId = this.profilesService.GetId(userId);
            var isOwner = this.groupsService.IsOwner(id, accessorId);

            if (isOwner && profileId != accessorId)
            {
                var isAdmin = this.groupsService.HasAdminPermissions(id, profileId);
                if (!isAdmin)
                {
                    await this.groupsService.ChangeProfileRole(profileId, id, GroupRole.Admin);
                }
                else
                {
                    await this.groupsService.ChangeProfileRole(profileId, id, GroupRole.Member);
                }
            }

            return this.RedirectToAction(nameof(this.GroupMembers), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Kick(int id, int profileId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessorId = this.profilesService.GetId(userId);
            var isAccessorOwner = this.groupsService.IsOwner(id, accessorId);

            if (isAccessorOwner && profileId != accessorId)
            {
                await this.groupsService.RemoveMember(profileId, id);
            }
            else
            {
                var isAccessorAdmin = this.groupsService.HasAdminPermissions(id, accessorId);
                var isProfileAdmin = this.groupsService.HasAdminPermissions(id, profileId);

                if (isAccessorAdmin && !isProfileAdmin)
                {
                    await this.groupsService.RemoveMember(profileId, id);
                }
            }

            return this.RedirectToAction(nameof(this.GroupMembers), new { id });
        }

        public IActionResult InviteToGroup(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            var isAdmin = this.groupsService.HasAdminPermissions(id, profileId);

            if (isAdmin)
            {
                var viewModel = new InviteFriendsViewModel()
                {
                    Id = id,
                    Friends = this.groupsService.FriendsToInvite(id, profileId),
                };

                return this.View(viewModel);
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int id, int profileId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accessorId = this.profilesService.GetId(userId);
            var isAdmin = this.groupsService.HasAdminPermissions(id, accessorId);

            if (isAdmin && this.profilesService.IsFriend(profileId, accessorId))
            {
                await this.groupsService.AddMember(profileId, id);

                return this.RedirectToAction(nameof(this.InviteToGroup), new { id });
            }

            return this.BadRequest();
        }

        public async Task<IActionResult> Leave(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            var isOwner = this.groupsService.IsOwner(id, profileId);

            if (!isOwner)
            {
                await this.groupsService.RemoveMember(profileId, id);
            }

            return this.RedirectToAction(nameof(this.List));
        }
    }
}
