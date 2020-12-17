namespace HiWorld.Web.ViewModels.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class InviteFriendsViewModel
    {
        public int Id { get; set; }

        public IEnumerable<GroupFriendAddViewModel> Friends { get; set; }
    }
}
