using HiWorld.Web.ViewModels.Friends;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public interface IFriendsService
    {
        Task SendFriendRequestAsync(int profileId, int senderId);

        Task RemoveFriendAsync(int profileId, int senderId);

        Task DenyFriendshipAsync(int id);

        Task AcceptFriendshipAsync(int id);

        IEnumerable<T> GetFriendRequests<T>(int profileId);

        IEnumerable<FriendViewModel> GetFriends(int profileId);
    }
}
