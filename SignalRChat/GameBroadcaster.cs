using Microsoft.AspNet.SignalR;
using ModelsLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SignalRChat
{
    public class GameBroadcaster
    {
        public static GameBroadcaster Instance => _instance.Value;

        private static readonly Lazy<GameBroadcaster> _instance =
              new Lazy<GameBroadcaster>(() => new GameBroadcaster());
        // We're going to broadcast to all clients a maximum of 25 times per second
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(40);
        private readonly IHubContext _hubContext;
        private Timer _movingBroadcastLoop;
        private Timer _objectStateBroadcasterLoop;
        private Timer _sceneUpdateBroadcasterLoop;

        public ConcurrentQueue<SyncObjectModel> _transfromsTemporaryStorage = new ConcurrentQueue<SyncObjectModel>();
        public ConcurrentQueue<SyncObjectModel> _objectsToCreate = new ConcurrentQueue<SyncObjectModel>();
        public ConcurrentQueue<SyncObjectModel> _objectsToDelete = new ConcurrentQueue<SyncObjectModel>();
        public ConcurrentQueue<SyncObjectModel> _objectsToUpdate = new ConcurrentQueue<SyncObjectModel>();

        public ConcurrentDictionary<string,SyncObjectModel[]> _sceneUpdate = new ConcurrentDictionary<string, SyncObjectModel[]>();

        public GameBroadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
            // Start the broadcast moving loop
            _movingBroadcastLoop = new Timer(
                BroadcastMoving,
                null,
                BroadcastInterval,
                BroadcastInterval);
            // Start the broadcast states loop
            _objectStateBroadcasterLoop = new Timer(
                BroadcastSceneCreate,
                null,
                BroadcastInterval,
                BroadcastInterval);
            _sceneUpdateBroadcasterLoop = new Timer(
                BroadcastSceneUpdate,
                null,
                BroadcastInterval,
                BroadcastInterval);
        }

        public void BroadcastMoving(object state)
        {
            foreach (var syncObjectModel in _objectsToUpdate)
            {
                _hubContext.Clients.Group(syncObjectModel.RoomModel.Id.ToString(),
                    syncObjectModel.UserModel.connectionId).GameBroadcaster_UpdateMoving(syncObjectModel);
            }
        }

        public void BroadcastSceneCreate(object state)
        {
            foreach (var item in _objectsToCreate)
            {
                _hubContext.Clients.Group(item.RoomModel.Id.ToString(), item.UserModel.connectionId).GameBroadcaster_CreateModel(item);
            }
            _objectsToCreate = new ConcurrentQueue<SyncObjectModel>();
        }

        public void BroadcastSceneDelete(object state)
        {
            foreach (var item in _objectsToCreate)
            {
                _hubContext.Clients.Group(item.RoomModel.Id.ToString()).GameBroadcaster_DeleteModel(item);
            }
            _objectsToDelete = new ConcurrentQueue<SyncObjectModel>();
        }

        public void BroadcastSceneUpdate(object state)
        {
            foreach (var sceneUpdate in _sceneUpdate)
            {
                _hubContext.Clients.All.UpdateSceneFor(sceneUpdate.Value);
            }
            _sceneUpdate = new ConcurrentDictionary<string, SyncObjectModel[]>();
        }

        /// <summary>
        /// User joining the room broadcasting to all room clients, exclude current user
        /// </summary>
        /// <param name="user"></param>
        public void UserJoinedRoom(UserModel user)
        {
            try
            {
                _hubContext.Clients.Group(user.Rooms.Id.ToString()).UserJoinRoomStatus(user);
            }
            catch (Exception e)
            {
                

            }
        }
        /// <summary>
        /// User leaving the room broadcasting to all room clients, exclude current user
        /// </summary>
        /// <param name="user"></param>
        public void UserLeavedRoom(UserModel user, string roomId)
        {
            _hubContext.Clients.Group(roomId).UserLeavedRoom(user);
        }

        public void FailedTaskMessage(string message, string connectionId)
        {
            _hubContext.Clients.Client(connectionId).FailedTaskMessage(message);
        }

    }
}
