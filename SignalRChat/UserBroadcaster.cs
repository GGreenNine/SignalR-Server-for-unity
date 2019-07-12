using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using ModelsLibrary;

namespace SignalRChat
{
    public class UserBroadcaster
    {
        public static UserBroadcaster Instance => _instance.Value;

        private static readonly Lazy<UserBroadcaster> _instance =
            new Lazy<UserBroadcaster>(() => new UserBroadcaster());

        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(40);

        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;

        public UserBroadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients

            _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserHub>();
        }

        /// <summary>
        /// User registration broadcast
        /// </summary>
        /// <param name="user"></param>
        public void RegisterUser(UserModel user)
        {
            _hubContext.Clients.Client(user.connectionId).RegistrateStatus(user);
        }
        /// <summary>
        /// User authorization broadcast
        /// </summary>
        /// <param name="user"></param>
        public void UserAuthorization(UserModel user)
        {
            _hubContext.Clients.Client(user.connectionId)
                .AuthorizationStatus(user);
        }

        public void FailedTaskMessage(string message, string connectionId)
        {
            _hubContext.Clients.Client(connectionId).FailedTaskMessage(message);
        }
    }
}