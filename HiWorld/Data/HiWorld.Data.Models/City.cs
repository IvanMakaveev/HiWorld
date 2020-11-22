namespace HiWorld.Data.Models
{
    using HiWorld.Data.Common.Models;

    public class City : BaseModel<int>
    {
        public string Name { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}
