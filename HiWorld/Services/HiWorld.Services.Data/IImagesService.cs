namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IImagesService
    {
        Task<string> Create(IFormFile image, string path);
    }
}
