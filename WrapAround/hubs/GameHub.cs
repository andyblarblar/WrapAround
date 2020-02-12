using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WrapAround.Logic.Entities;

namespace WrapAround.hubs
{
    public class GameHub : Hub
    {
        /// <summary>
        /// The singleton that manages the state of all lobbies.
        /// </summary>
        private readonly IServerLoop serverLoop;

        /// <summary>
        /// To fix my spgett code
        /// </summary>
        private readonly IUserGameRepository userGameRepository;

        public GameHub(IServerLoop serverLoop, IUserGameRepository userGameRepository)
        {
            this.serverLoop = serverLoop;
            this.userGameRepository = userGameRepository;
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
                userGameRepository.UserDictionary.Add(Context.ConnectionId,$"lobby{gameId}");
            }
            
        }

        /// <summary>
        /// Updates the location of a player.
        /// </summary>
        /// <param name="player">a complete representation of the players state, as reported by the client.</param>
        /// <returns></returns>
        public async Task UpdatePlayerPosition(Paddle player)
        {
            await serverLoop.UpdatePlayerPosition(player);
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

        //TODO create "ping of death" to clean out player that dont respond

        /// <summary>
        /// Hot disconnects a user
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
           await Groups.RemoveFromGroupAsync(Context.ConnectionId, userGameRepository.UserDictionary[Context.ConnectionId]);
            userGameRepository.UserDictionary.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        

    }
}
