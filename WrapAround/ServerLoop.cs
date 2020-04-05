using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WrapAround.hubs;
using WrapAround.Logic;
using WrapAround.Logic.Entities;
using WrapAround.Logic.Interfaces;

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
        private const int BroadcastInterval = 17;

        /// <summary>
        /// Holds the states of Contexts.
        /// </summary>
        private readonly List<GameContext> _gameContextList;

        private const int MaxLobbyCount = 4;

        public ServerLoop(IHubContext<GameHub> hubContext, IMapLoader mapLoader)
        {
            _gameContextList = new List<GameContext>(MaxLobbyCount);
            var maps = mapLoader.LoadMaps();

            for (var i = 0; i < MaxLobbyCount; i++)
            {
                _gameContextList.Add(new GameContext(id: i, maps: maps));
            }

            var broadCastLoop = new Timer(BroadcastInterval);

            //Every 17ms, update all lobbies in parallel and then send to clients.
            broadCastLoop.Elapsed += (sender, args) =>
            {
                Parallel.ForEach(_gameContextList, async context =>
                {
                    await context.Update();

                    await hubContext.Clients.Group($"lobby{context.Id}").SendAsync("ReceiveContextUpdate", context);//send to frontend

                });

            };

            broadCastLoop.AutoReset = true;//TODO this might not be working as intended, might be sending wayyyy to much.
            broadCastLoop.Start();

        }


        /// <summary>
        /// Adds a player to a lobby.
        /// </summary>
        /// <param name="gameId">the lobby to be added to.</param>
        /// <param name="playerIsOnRight">which side the paddle is on</param>
        /// <returns>the player Id given.</returns>
        public async Task<int> AddPlayer(int gameId, bool playerIsOnRight, string hash)
        {
            return await Task.Run(async () =>
            {
                var lobby = _gameContextList.First(context => context.Id == gameId);
                return await lobby.AddPlayer(playerIsOnRight, hash);

            });

        }

        /// <summary>
        /// Removes a player from a lobby.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task RemovePlayer(Paddle player)
        {
            await Task.Run((async () =>
            {
                var context = _gameContextList.First(gameContext => gameContext.Id == player.GameId);
                await context.RemovePlayer(player);

            }));


        }

        /// <summary>
        /// total players of each lobby as a list
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetLobbyPlayerCounts()
        {
            return await Task.Run(() =>
            {
                return _gameContextList.Select(game => game.Players.Count).ToList();
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
                var context = _gameContextList.First(gameContext => gameContext.Id == player.GameId);
                var serverPlayer = context.Players.First(paddle => paddle.Id == player.Id);
                if (serverPlayer.Hash == player.Hash) serverPlayer.Update(player.Position);

            });

        }




    }
}
