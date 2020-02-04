using System.Collections.Generic;

namespace WrapAround.Logic.Interfaces
{
    /// <summary>
    /// Represents a class that can load game maps into memory.
    /// </summary>
    public interface IMapLoader
    {
        List<GameMap> LoadMaps(string path = @".\gameMaps\");

    }


}