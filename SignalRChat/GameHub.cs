using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ModelsLibrary;

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

        public void CreateModel(SyncObjectModel clientModel)
        {
            using (var db = new MultiplayerServerDB())
            {
                db.Models.Add(clientModel);
                db.SaveChangesAsync();
            }

            _broadcaster.CreateModel(clientModel);
        }
    }
}