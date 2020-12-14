using HiWorld.Web.ViewModels.Groups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public interface IMessagesService
    {
        Task DeleteMessage(int messageId);

        bool IsOwner(int messageId, int profileId);

        int GetMessageGroupId(int id);

        Task<int> AddMessage(int groupId, int profileId, string text);

        T GetById<T>(int id);
    }
}
