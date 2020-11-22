namespace HiWorld.Data.Models
{
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;

    public class Group : BaseDeletableModel<int>
    {
        public Group()
        {
            this.GroupMembers = new HashSet<GroupMember>();
            this.Messages = new HashSet<Message>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public virtual ICollection<GroupMember> GroupMembers { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
