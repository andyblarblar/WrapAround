using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WrapAround.hubs;
using WrapAround.Logic;
using WrapAround.Logic.Entities;

namespace WrapAround
{
    public class ServerLoop : IServerLoop
    {
         
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



        /// <summary>
        /// Adds a player to a lobby.
        /// </summary>
        /// <param name="gameId">the lobby to be added to.</param>
        /// <returns>the player Id given.</returns>
        public async Task<int> AddPlayer(int gameId)
        {
            return await Task.Run(async () =>
            {
                var lobby = gameContextList.FirstOrDefault(context => context.id == gameId);
                return await lobby.AddPlayer();

            });

        }


        /// <summary>
        /// Updates players paddle position
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task UpdatePlayerPosition(Paddle player)
        {
            await Task.Run(() =>
            {
                var context = gameContextList.FirstOrDefault(gameContext => gameContext.id == player.gameId);
                context.players.FirstOrDefault(paddle => paddle.id == player.id).position = player.position;

            });

        }




    }
}
