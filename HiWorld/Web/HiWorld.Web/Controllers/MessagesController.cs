using HiWorld.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiWorld.Web.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly IMessagesService messagesService;
        private readonly IGroupsService groupsService;

        public MessagesController(
            IProfilesService profilesService,
            IMessagesService messagesService,
            IGroupsService groupsService)
        {
            this.profilesService = profilesService;
            this.messagesService = messagesService;
            this.groupsService = groupsService;
        }

        [HttpPost]
        public async Task Delete(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profileId = this.profilesService.GetId(userId);
            var isMessageOwner = this.messagesService.IsOwner(id, profileId);

            if (isMessageOwner)
            {
                await this.messagesService.DeleteMessage(id);
            }
            else
            {
                var groupId = this.messagesService.GetMessageGroupId(id);
                var isAdmin = this.groupsService.HasAdminPermissions(groupId, profileId);
                if (isAdmin)
                {
                    await this.messagesService.DeleteMessage(id);
                }
            }
        }
    }
}
