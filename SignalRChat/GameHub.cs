using System;
using System.Collections.Generic;
using System.Linq;
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
                var dbRoom = db.Rooms.Find(userModel.RoomModelId);

                dbModel.PlayerId = dbUser.PlayerId;
                dbModel.RoomModelId = dbRoom.Id;

                db.SaveChanges();

                _broadcaster._objectsToCreate.Enqueue(dbModel);
            }

        }

        public void DeleteModel(SyncObjectModel clientModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                db.Models.Remove(clientModel);
                db.SaveChangesAsync();
            }

            _broadcaster._objectsToDelete.Enqueue(clientModel);
        }
    }
}