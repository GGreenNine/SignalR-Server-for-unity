using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using ModelsLibrary;
using Newtonsoft.Json;

namespace SignalRChat
{
    public class MyHub : Hub
    {
        public void Send(DefaultChatModel model)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcast(model);
        }
    }
}