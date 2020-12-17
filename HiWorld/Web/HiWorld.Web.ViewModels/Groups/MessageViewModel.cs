namespace HiWorld.Web.ViewModels.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using AutoMapper;
    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class MessageViewModel : IMapFrom<Message>
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int ProfileId { get; set; }

        public string ProfileFirstName { get; set; }

        public string ProfileLastName { get; set; }

        public DateTime CreatedOn { get; set; }

        [IgnoreMap]
        public string CreatedOnString => this.CreatedOn.ToString("dd/MM/yyyy HH:mm");
    }
}
