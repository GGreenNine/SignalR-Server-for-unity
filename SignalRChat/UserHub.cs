using System;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ModelsLibrary;

namespace SignalRChat
{
    [HubName("UserHub")]
    public class UserHub : Hub
    {
        /// <summary>
        /// Ссылка на наш бродкастер
        /// </summary>
        private UserBroadcaster _broadcaster;
        public UserHub()
            : this(UserBroadcaster.Instance)
        {
        }
        public UserHub(UserBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }
        public void RegisterUser(UserModel user)
        {
            user.connectionId = Context.ConnectionId;
            using (var db = new MultiplayerServerDB())
            {
                try
                {
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    _broadcaster.RegisterUser(user);
                }
                catch (Exception e)
                {
                    var s = e.Message;
                }
            }
        }

        public void UserAuthorization(UserModel user)
        {
            user.connectionId = Context.ConnectionId;
            using (var db = new MultiplayerServerDB())
            {
                user.PlayerId = db.Users.SingleOrDefault((x) => x.UserName == user.UserName).PlayerId;
                _broadcaster.UserAuthorization(db.Users.ToArray().Contains(user) ? user : null);
            }
        }



    }

}