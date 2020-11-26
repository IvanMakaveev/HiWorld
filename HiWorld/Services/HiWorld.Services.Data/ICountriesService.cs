using System.Collections.Generic;

namespace HiWorld.Services.Data
{
    public interface ICountriesService
    {
        IEnumerable<KeyValuePair<int, string>> GetAllAsKvp();
    }
}
