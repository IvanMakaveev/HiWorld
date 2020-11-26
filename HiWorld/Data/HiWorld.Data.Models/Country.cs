namespace HiWorld.Data.Models
{
    using System.Collections.Generic;

    using HiWorld.Data.Common.Models;

    public class Country : BaseModel<int>
    {
        public string Name { get; set; }
    }
}
