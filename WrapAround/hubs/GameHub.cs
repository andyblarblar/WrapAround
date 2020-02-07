using System;
using System.Collections.Generic;
using System.Linq;
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

        public GameHub(IServerLoop serverLoop)
        {
            this.serverLoop = serverLoop;

        }


        /// <summary>
        /// Adds a player to a lobby.
        /// </summary>
        /// <param name="gameId">the lobby to join</param>
        /// <param name="isOnRight">which side the paddle will be on</param>
        /// <returns>the id given, -1 if lobby is full.</returns>
        public async Task AddPlayer(int gameId, bool isOnRight)
        {
            var id = await serverLoop.AddPlayer(gameId, isOnRight);
            await Clients.Caller.SendAsync("ReceiveId", id);

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

        public async Task GetLobbyPlayerCounts()
        {
            var lobbyCounts = await serverLoop.GetLobbyPlayerCounts();

            await Clients.Caller.SendAsync("ReceiveLobbyCounts", lobbyCounts);

        }  





    }
}
