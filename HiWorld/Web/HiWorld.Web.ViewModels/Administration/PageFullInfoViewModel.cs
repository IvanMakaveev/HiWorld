namespace HiWorld.Web.ViewModels.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HiWorld.Data.Models;
    using HiWorld.Services.Mapping;

    public class PageFullInfoViewModel : IMapFrom<Page>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ImageId { get; set; }

        public int ProfileId { get; set; }
    }
}
