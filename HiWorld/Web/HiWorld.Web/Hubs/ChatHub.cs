using HiWorld.Services.Data;
using HiWorld.Web.ViewModels.Groups;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiWorld.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IProfilesService profilesService;
        private readonly IGroupsService groupsService;
        private readonly IMessagesService messagesService;

        public ChatHub(
            IProfilesService profilesService,
            IGroupsService groupsService,
            IMessagesService messagesService)
        {
            this.profilesService = profilesService;
            this.groupsService = groupsService;
            this.messagesService = messagesService;
        }

        public async Task ConnectToChat(int groupId)
        {
            var connection = this.Context.ConnectionId;
            await this.Groups.AddToGroupAsync(connection, groupId.ToString());
        }

        public async Task Send(int groupId, string message)
        {
            var userId = this.Context.UserIdentifier;
            var profileId = this.profilesService.GetId(userId);
            var isMember = this.groupsService.IsMember(groupId, profileId);

            if (isMember)
            {
                var messageId = await this.messagesService.AddMessage(groupId, profileId, message);
                var messageModel = this.messagesService.GetById<MessageViewModel>(messageId);
                await this.Clients.Group(groupId.ToString()).SendAsync("RecieveMessage", messageModel);
            }
        }

        public async Task Delete(int groupId, int messageId)
        {
            var userId = this.Context.UserIdentifier;
            var profileId = this.profilesService.GetId(userId);
            var isMessageOwner = this.messagesService.IsOwner(messageId, profileId);

            if (isMessageOwner)
            {
                await this.messagesService.DeleteMessage(messageId);
                await this.Clients.Group(groupId.ToString()).SendAsync("DeleteMessage", messageId);
            }
            else
            {
                var isAdmin = this.groupsService.HasAdminPermissions(groupId, profileId);
                if (isAdmin)
                {
                    await this.messagesService.DeleteMessage(messageId);
                    await this.Clients.Group(groupId.ToString()).SendAsync("DeleteMessage", messageId);
                }
            }
        }
    }
}
