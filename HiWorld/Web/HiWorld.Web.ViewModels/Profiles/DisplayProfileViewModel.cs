namespace HiWorld.Web.ViewModels.Profiles
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DisplayProfileViewModel
    {
        public int Id { get; set; }

        public bool IsOwner { get; set; }

        public bool IsFriend { get; set; }

        public bool IsPending { get; set; }

        public bool IsFollowing { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public string ImagePath { get; set; }

        public string Country { get; set; }

        public int FriendsCount { get; set; }

        public int FollowersCount { get; set; }

        public ICollection<ProfilePostViewModel> Posts { get; set; }
    }
}
