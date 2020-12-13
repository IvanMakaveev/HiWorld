namespace HiWorld.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITagsService
    {
        Task<int> GetIdAsync(string text);

        string GetName(int id);

        IEnumerable<T> SearchPagesByTag<T>(int id);

        IEnumerable<T> SearchPostsByTag<T>(int id, int pageNumber, int count = 20);

        int SearchPostsByTagCount(int id);
    }
}
