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
                if (db.Users.ToArray().Contains(user))
                {
                    _broadcaster.RegisterUser(null, Context.ConnectionId);
                    return;
                }
                db.Users.Add(user);
                db.SaveChanges();
                _broadcaster.RegisterUser(user);
            }
        }

        public void UserAuthorization(UserModel user)
        {
            user.connectionId = Context.ConnectionId;
            using (var db = new MultiplayerServerDB())
            {
                _broadcaster.UserAuthorization(db.Users.ToArray().Contains(user) ? user : null);
            }
        }

        public async Task JoinRoom(UserModel user)
        {
            using (var db = new MultiplayerServerDB())
            {
                int? roomMaxId = db.Rooms.Max(x => x.Id);
                var room = db.Rooms.Find(roomMaxId);
                var dbUser = db.Users.Find(user.UserName);

                if (room != null)
                {
                    dbUser.RoomModelId = room.Id;
                    db.SaveChangesAsync();
                    await Groups.Add(Context.ConnectionId, roomMaxId.ToString());
                    _broadcaster.UserJoinedRoom(dbUser);
                }
                else
                {
                    db.Rooms.Add(new RoomModel());
                    db.SaveChanges();
                    await JoinRoom(user);
                }
            }
        }

        public void LeaveRoom(UserModel user)
        {
            using (var db = new MultiplayerServerDB())
            {
                var userDb = db.Users.Find(user.UserName);
                if (userDb == null)
                {
                    _broadcaster.FailedTaskMessage("No such user found", user.connectionId);
                    return;
                }
                var roomId = userDb.RoomModelId;
                userDb.RoomModelId = null;
                db.SaveChanges();
                _broadcaster.UserLeavedRoom(userDb, roomId.ToString());
            }
            Groups.Remove(Context.ConnectionId, user.RoomModelId.ToString());
        }

    }

}