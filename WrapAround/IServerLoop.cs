using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;

namespace WrapAround
{
    /// <summary>
    /// the interface used for dependance injection of the game runtime
    /// </summary>
    public interface IServerLoop
    {
        Task UpdatePlayerPosition(Paddle player);
        Task<int> AddPlayer(int gameId, bool paddleIsOnRight);

        Task<List<int>> GetLobbyPlayerCounts();


    }
}
