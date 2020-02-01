using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WrapAround.hubs;

namespace WrapAround
{
    public class ServerLoop
    {
        private readonly TimeSpan broadcastInterval = TimeSpan.FromMilliseconds(16);
        private readonly IHubContext<GameHub> hubContext;






    }
}
