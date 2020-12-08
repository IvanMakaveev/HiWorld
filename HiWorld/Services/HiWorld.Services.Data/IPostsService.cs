using HiWorld.Web.ViewModels.Posts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public interface IPostsService
    {
        Task CreateForProfile(int id, CreatePostInputModel input, string path);

        Task LikePost(int profileId, int id);
    }
}
