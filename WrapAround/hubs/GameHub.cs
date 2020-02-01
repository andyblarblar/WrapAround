using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WrapAround.hubs
{
    public class GameHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            //TODO handle a user disconnecting
            return base.OnConnectedAsync();
        }





    }
}
