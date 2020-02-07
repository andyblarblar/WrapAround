using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WrapAround.hubs;
using WrapAround.Logic;
using WrapAround.Logic.Entities;
using WrapAround.Logic.Interfaces;
using WrapAround.Logic.Util;

namespace WrapAround
{
    /// <summary>
    /// The high level singleton that manages changes to game state
    /// </summary>
    public class ServerLoop : IServerLoop
    {
         
        /// <summary>
        /// The speed at which the server will send updates to clients. ~60fps
        /// </summary>
        private readonly int broadcastInterval = 17;
        private readonly IHubContext<GameHub> hubContext;

        /// <summary>
        /// the global ticker that steps the state and sends game contexts to clients on a timer
        /// </summary>
        private Timer broadCastLoop;

        /// <summary>
        /// Holds the states of Contexts.
        /// </summary>
        private List<GameContext> gameContextList;

        private const int MAX_LOBBY_COUNT = 4;

        public ServerLoop(IHubContext<GameHub> hubContext, IMapLoader mapLoader)
        {
            this.hubContext = hubContext;
            gameContextList = new List<GameContext>(MAX_LOBBY_COUNT);
            var maps = mapLoader.LoadMaps();

            for (var i = 0; i < MAX_LOBBY_COUNT; i++)
            {
                gameContextList.Add(new GameContext(id: i, maps: maps));
            }

            broadCastLoop = new Timer(broadcastInterval);

            //Every 17ms, update all lobbies in parallel and then send to clients.
            broadCastLoop.Elapsed += (sender, args) =>
            { 
                Parallel.ForEach(gameContextList, async context =>
                {
                    await context.Update();
                    await hubContext.Clients.All.SendAsync("ReceiveGameContext",context);//send to frontend
                });

            };

            broadCastLoop.AutoReset = true;
            broadCastLoop.Start();

        }


        /// <summary>
        /// Adds a player to a lobby.
        /// </summary>
        /// <param name="gameId">the lobby to be added to.</param>
        /// <param name="playerIsOnRight">which side the paddle is on</param>
        /// <returns>the player Id given.</returns>
        public async Task<int> AddPlayer(int gameId, bool playerIsOnRight)
        {
            return await Task.Run(async () =>
            {
                var lobby = gameContextList.FirstOrDefault(context => context.id == gameId);
                return await lobby.AddPlayer(playerIsOnRight);

            });

        }
        
        /// <summary>
        /// total players of each lobby as a list
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetLobbyPlayerCounts()
        {
           return await Task.Run((() =>
           {
              return gameContextList.Select(game => game.players.Count).ToList();
           }));

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
