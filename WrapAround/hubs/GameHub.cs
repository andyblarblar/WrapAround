using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WrapAround.hubs
{
    public class GameHub : Hub
    {
        private ServerLoop serverLoop;

        public GameHub() : this(ServerLoop.)
        public GameHub(ServerLoop serverLoop)
        {
            this.serverLoop = serverLoop;
        }

        public override Task OnConnectedAsync()
        {
            //TODO handle a user disconnecting
            return base.OnConnectedAsync();
        }





    }
}
