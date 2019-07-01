using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public void CreateModel(SyncObjectModel clientModel, UserModel userModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                db.Models.Add(clientModel);
                db.SaveChanges();

                /*
                    * Ищем нашу модель в базе,
                    * После чего присваиваем ей данные о юзере и комнате
                    * Если сделать это раньше, например передвать модель уже с заданными параметрами
                    * База данных не отразит данную информацию как положено
                    * Она не поймет связей, получается нужно сделать 2 лишнии операции
                    * todo Подумать как это исправить!
                    */

                var dbModel = db.Models.First(x => x.ModelId == clientModel.ModelId);
                var dbUser = db.Users.Find(userModel.PlayerId);
                var dbRoom = db.Rooms.Find(dbUser.RoomModelId);

                dbModel.PlayerId = dbUser.PlayerId;
                dbModel.RoomModelId = dbRoom.Id;

                db.SaveChanges();

                _broadcaster._objectsToCreate.Enqueue(dbModel);
            }

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

        public async Task  UpdateScene(UserModel userModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                var dbUser = db.Users.Find(userModel.PlayerId);
                var sceneObjects = db.Models.Where(x => x.Rooms.Id == dbUser.RoomModelId).Select(x => x).ToArrayAsync();
                await sceneObjects;
                _broadcaster._sceneUpdate.TryAdd(dbUser.connectionId, sceneObjects.Result);
                var test = _broadcaster._sceneUpdate;
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
                int? roomMaxId = db.Rooms.Max(x => x.Id);
                var room = db.Rooms.Find(roomMaxId);
                var dbUser = db.Users.Find(userModel.PlayerId);

                if (room != null)
                {
                    dbUser.RoomModelId = room.Id;
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
                var roomId = userDb.RoomModelId;
                userDb.RoomModelId = null;
                db.SaveChanges();
                _broadcaster.UserLeavedRoom(userDb, roomId.ToString());
            }
            Groups.Remove(Context.ConnectionId, user.RoomModelId.ToString());
        }

    }
}