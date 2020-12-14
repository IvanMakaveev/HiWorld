using System;
using System.Collections.Generic;
using System.Text;

namespace HiWorld.Web.ViewModels.Groups
{
    public class InviteFriendsViewModel
    {
        public int Id { get; set; }

        public IEnumerable<GroupFriendAddViewModel> Friends { get; set; }
    }
}
