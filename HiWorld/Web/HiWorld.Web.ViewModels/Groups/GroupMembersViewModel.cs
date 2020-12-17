namespace HiWorld.Web.ViewModels.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class GroupMembersViewModel
    {
        public int Id { get; set; }

        public IEnumerable<MemberInfoViewModel> Members { get; set; }

        public bool IsOwner { get; set; }

        public bool IsAdmin { get; set; }
    }
}
