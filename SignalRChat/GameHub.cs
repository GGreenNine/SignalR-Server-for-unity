using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ModelsLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SignalRChat
{
    [HubName("GameHub")]
    public class GameHub : Hub
    {
        /// <summary>
        /// Ссылка на наш бродкастер
        /// </summary>
        private GameBroadcaster _broadcaster;
        public GameHub()
            : this(GameBroadcaster.Instance)
        {
        }
        public GameHub(GameBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }
        /// <summary>
        /// Записываем присланную модель в базу данных
        /// И передаем бродкастеру
        /// </summary>
        /// <param name="clientModel"></param>
        /// <param name="userModel"></param>
        public void CreateModel(SyncObjectModel clientModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                db.Models.Add(clientModel);
                db.SaveChanges();

                var dbModel = db.Models.Find(clientModel.ModelId);

                var test1 = db.Rooms.Include(x => x.Users).Include(x => x.Models).ToList(); 

                _broadcaster._objectsToCreate.Enqueue(dbModel);
            }
        }

        public void UpdateMoving(SyncObjectModel clientModel)
        {
            _broadcaster._objectsToUpdate.Enqueue(clientModel);
        }
        /// <summary>
        /// Удаляем модель из базы данных
        /// И передаем операцию бродкастеру
        /// </summary>
        /// <param name="clientModel"></param>
        public void DeleteModel(SyncObjectModel clientModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                db.Models.Remove(clientModel);
                db.SaveChangesAsync();
            }

            _broadcaster._objectsToDelete.Enqueue(clientModel);
        }

        public async Task UpdateScene(UserModel userModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                //var dbUser = db.Users.Find(userModel.PlayerId);
                //var sceneObjects = db.Models.Where(x => x.RoomModel.RoomModelId == dbUser.RoomModelId).Select(x => x).ToArrayAsync();
                //await sceneObjects;
                //_broadcaster._sceneUpdate.TryAdd(dbUser.connectionId, sceneObjects.Result);
                //var test = _broadcaster._sceneUpdate;
            }
        }

        /// <summary>
        /// Добавляем пользователя в команту
        /// Или создаем эту комнату
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task JoinRoom(UserModel userModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                var dbUser = db.Users.Find(userModel.UserName);

                if (db.Rooms.Any())
                {
                    var roomMaxId = db.Rooms.Max(x=> x.Id);
                    var testRoom = db.Rooms.Find(roomMaxId);

                    dbUser.RoomModelId = roomMaxId;
                    db.SaveChanges();
                    await Groups.Add(Context.ConnectionId, roomMaxId.ToString());
                    _broadcaster.UserJoinedRoom(dbUser);
                }
                else
                {
                    db.Rooms.Add(new RoomModel());
                    db.SaveChanges();
                    await JoinRoom(userModel);
                }
            }
        }
        /// <summary>
        /// Удаляем пользователя из комнаты,
        /// </summary>
        /// <param name="user"></param>
        public void LeaveRoom(UserModel user)
        {
            using (var db = new MultiplayerServerDB())
            {

                var userDb = db.Users.Find(user.UserName);
                if (userDb == null)
                {
                    _broadcaster.FailedTaskMessage("No such userModel found", user.connectionId);
                    return;
                }
                _broadcaster.UserLeavedRoom(userDb, userDb.RoomModelId.ToString());
                /*
                 * Проверяем, если пользователь, покинувший комнату
                 * был последним пользователем в команате, удалем её. /todo каскадное удаление.
                 */
                var dbRoom = db.Rooms.FirstOrDefault(x => x.Id == user.RoomModelId);
                if (dbRoom != null && dbRoom.Users.Count == 1)
                {
                    db.Rooms.Remove(dbRoom);
                    userDb.RoomModelId = null;
                    Groups.Remove(Context.ConnectionId, user.RoomModelId.ToString());
                }
                else
                    userDb.RoomModelId = null;

                db.SaveChanges();
            }
            Groups.Remove(Context.ConnectionId, user.RoomModelId.ToString());
        }

    }
}