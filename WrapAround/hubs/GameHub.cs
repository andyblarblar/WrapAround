using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;
using WrapAround.Logic.Util;

namespace WrapAround.hubs
{
    public class GameHub : Hub
    {
        /// <summary>
        /// The singleton that manages the state of all lobbies.
        /// </summary>
        private readonly IServerLoop _serverLoop;

       

        private readonly ILogger<GameHub> logger;

        public GameHub(IServerLoop serverLoop, ILogger<GameHub> logger)
        {
            this._serverLoop = serverLoop;
            
            this.logger = logger;
        }


        /// <summary>
        /// Adds a player to a lobby.
        /// </summary>
        /// <param name="gameId">the lobby to join</param>
        /// <param name="isOnRight">which side the paddle will be on</param>
        /// <param name="hash">a unique hash generated and stored on the client used for authorization</param>
        /// <returns>the id given, -1 if lobby is full.</returns>
        public async Task AddPlayer(int gameId, bool isOnRight, string hash)
        {
            var id = await _serverLoop.AddPlayer(gameId, isOnRight, hash);
            await Clients.Caller.SendAsync("ReceiveId", id);
            Console.WriteLine($"sent {id} to ma dude");
            if (id != -1)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"lobby{gameId}");
            }

        }

        /// <summary>
        /// Updates the location of a player.
        /// </summary>
        /// <returns></returns>
        public async Task UpdatePlayerPosition(string hash, float posX,float posY, int gameId, int Id)
        {
            var position = new Vector2(posX,posY);
            await _serverLoop.UpdatePlayerPosition(new Paddle(){GameId = gameId, Position = position, Hash = hash, Id = Id});
        }

        /// <summary>
        /// Sends the lobby player counts to the caller as a list of int
        /// </summary>
        /// <returns></returns>
        public async Task GetLobbyPlayerCounts()
        {
            var lobbyCounts = await _serverLoop.GetLobbyPlayerCounts();

            await Clients.Caller.SendAsync("ReceiveLobbyCounts", lobbyCounts);

        }

        /// <summary>
        /// Gracefully removes a player from a lobby.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task RemovePlayerFromLobby(Paddle player)
        {
            await _serverLoop.RemovePlayer(player);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"lobby{player.GameId}");
        }


        public async Task Ping()
        {
            Console.WriteLine("ping");
            await Clients.Caller.SendAsync("pong");
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("connected");
            return base.OnConnectedAsync();
        }
    }
}
