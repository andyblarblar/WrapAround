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
        private readonly IServerLoop serverLoop;

        /// <summary>
        /// To fix a circular ascendancy
        /// </summary>
        private readonly IUserGameRepository userGameRepository;

        private readonly ILogger<GameHub> logger;

        public GameHub(IServerLoop serverLoop, IUserGameRepository userGameRepository, ILogger<GameHub> logger)
        {
            this.serverLoop = serverLoop;
            this.userGameRepository = userGameRepository;
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
            var id = await serverLoop.AddPlayer(gameId, isOnRight, hash);
            await Clients.Caller.SendAsync("ReceiveId", id);
            if (id != -1)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"lobby{gameId}");

                try
                {
                    userGameRepository.UserDictionary.Add(Context.ConnectionId, $"lobby{gameId}");
                }
                catch (Exception e)
                {
                   //Will error here if same person connects twice, we need this for debugging
                }

            }

        }

        /// <summary>
        /// Updates the location of a player.
        /// </summary>
        /// <param name="player">a complete representation of the players state, as reported by the client.</param>
        /// <returns></returns>
        public async Task UpdatePlayerPosition(string hash, Vector2 position, int gameId, int Id)
        {
            await serverLoop.UpdatePlayerPosition(new Paddle(){GameId = gameId, Position = position, Hash = hash, Id = Id});
        }

        /// <summary>
        /// Sends the lobby player counts to the caller as a list of int
        /// </summary>
        /// <returns></returns>
        public async Task GetLobbyPlayerCounts()
        {
            var lobbyCounts = await serverLoop.GetLobbyPlayerCounts();

            await Clients.Caller.SendAsync("ReceiveLobbyCounts", lobbyCounts);

        }

        /// <summary>
        /// Gracefully removes a player from a lobby.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task RemovePlayerFromLobby(Paddle player)
        {
            await serverLoop.RemovePlayer(player);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"lobby{player.GameId}");
        }



        /// <summary>
        /// Hot disconnects a user
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, userGameRepository.UserDictionary[Context.ConnectionId]);
                userGameRepository.UserDictionary.Remove(Context.ConnectionId);
            }
            catch (Exception e)
            {
                // ignored
            }


            return base.OnDisconnectedAsync(exception);
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
