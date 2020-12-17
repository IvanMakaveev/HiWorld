namespace HiWorld.Web.ViewModels.Friends
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class FriendViewModel
    {
        public int FriendId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
