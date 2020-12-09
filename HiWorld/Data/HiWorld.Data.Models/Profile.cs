namespace HiWorld.Data.Models
{
    using System;
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;
    using HiWorld.Data.Models.Enums;

    public class Profile : BaseDeletableModel<int>
    {
        public Profile()
        {
            this.PostLikes = new HashSet<PostLike>();
            this.Comments = new HashSet<Comment>();
            this.CommentLikes = new HashSet<CommentLike>();
            this.Posts = new HashSet<Post>();
            this.Following = new HashSet<ProfileFollower>();
            this.Followers = new HashSet<ProfileFollower>();
            this.FriendsRecieved = new HashSet<ProfileFriend>();
            this.FriendsSent = new HashSet<ProfileFriend>();
            this.Pages = new HashSet<Page>();
            this.PageFollows = new HashSet<PageFollower>();
            this.GroupMembers = new HashSet<GroupMember>();
            this.Messages = new HashSet<Message>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public string ImageId { get; set; }

        public virtual Image Image { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<PostLike> PostLikes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<CommentLike> CommentLikes { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<ProfileFollower> Followers { get; set; }

        public virtual ICollection<ProfileFollower> Following { get; set; }

        public virtual ICollection<ProfileFriend> FriendsSent { get; set; }

        public virtual ICollection<ProfileFriend> FriendsRecieved { get; set; }

        public virtual ICollection<Page> Pages { get; set; }

        public virtual ICollection<PageFollower> PageFollows { get; set; }

        public virtual ICollection<GroupMember> GroupMembers { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
