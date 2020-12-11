namespace HiWorld.Services.Data
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface IImagesService
    {
        Task<string> Create(IFormFile image, string path);
    }
}
