namespace HiWorld.Data.Models
{
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;

    public class Country : BaseModel<int>
    {
        public Country()
        {
            this.Cities = new HashSet<City>();
        }

        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
