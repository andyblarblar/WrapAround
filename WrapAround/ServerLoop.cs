using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WrapAround.hubs;
using WrapAround.Logic;

namespace WrapAround
{
    public class ServerLoop
    {
        /// <summary>
        /// the instance of ServerLoop used in the transient hub, Lazy instantiated.  
        /// </summary>
        public static readonly Lazy<ServerLoop> instance = new Lazy<ServerLoop>(() => new ServerLoop());//TODO inject
        /// <summary>
        /// The speed at which the server will send updates to clients.
        /// </summary>
        private readonly TimeSpan broadcastInterval = TimeSpan.FromMilliseconds(16);
        private readonly IHubContext<GameHub> hubContext;
        private Timer broadCastLoop;
        /// <summary>
        /// Holds the states of Contexts.
        /// </summary>
        private List<GameContext> gameContextList;

        public ServerLoop(IHubContext<GameHub> hubContext)
        {
            this.hubContext = hubContext;




        }





    }
}
