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

        private ConcurrentQueue<SyncObjectModel> _transfromsTemporaryStorage = new ConcurrentQueue<SyncObjectModel>();
        private ConcurrentQueue<SyncObjectModel> _objectsToCreate = new ConcurrentQueue<SyncObjectModel>();
        private ConcurrentQueue<SyncObjectModel> _objectsToDelete = new ConcurrentQueue<SyncObjectModel>();

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
                BroadcastObjectsState,
                null,
                BroadcastInterval,
                BroadcastInterval);
        }
        public void BroadcastMoving(object state)
        {
            foreach (var item in _transfromsTemporaryStorage)
            {
                _hubContext.Clients.AllExcept(item.UserModelId).GameBroadcaster_UpdateTransforms(item);
            }
            _transfromsTemporaryStorage = new ConcurrentQueue<SyncObjectModel>();
        }

        public void BroadcastObjectsState(object state)
        {
            foreach (var item in _objectsToCreate)
            {
                _hubContext.Clients.AllExcept(item.UserModelId).GameBroadcaster_CreateModel(item);
            }
            _objectsToCreate = new ConcurrentQueue<SyncObjectModel>();
        }

        public void CreateModel(SyncObjectModel model)
        {
            _objectsToCreate.Enqueue(model);
        }


    }
}
