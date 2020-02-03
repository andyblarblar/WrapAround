using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WrapAround.Logic.Entities;

namespace WrapAround
{
    public interface IServerLoop
    {
        Task UpdatePlayerPosition(Paddle player);
        Task<int> AddPlayer(int gameId);




    }
}
