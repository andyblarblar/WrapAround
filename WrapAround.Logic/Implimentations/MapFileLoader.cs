using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace WrapAround.Logic.Implimentations
{
    /// <summary>
    /// A simple map loader that just reads from a Json rep on disk
    /// </summary>
    public class MapFileLoader : Interfaces.IMapLoader
    {
        /// <summary>
        /// Loads maps from this objects mapPath property in parallel 
        /// </summary>
        /// <returns>all deserialized GameMaps</returns>
        public List<GameMap> LoadMaps(string dirPath = @".\gameMaps\")
        {
            var dir = Directory.GetFiles(dirPath);

            var maps = from file in dir.AsParallel()
                where file.EndsWith(".wamap")
                select JsonSerializer.Deserialize<GameMap>(File.ReadAllText(file));

            return maps.ToList();

        }
    }
}