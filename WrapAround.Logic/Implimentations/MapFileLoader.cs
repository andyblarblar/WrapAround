using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace WrapAround.Logic.Implimentations
{
    /// <summary>
    /// A simple map loader that just reads from a JSON serialized rep on disk
    /// </summary>
    public class MapFileLoader : Interfaces.IMapLoader
    {
        /// <summary>
        /// Loads maps from this objects mapPath property in parallel 
        /// </summary>
        /// <returns>all deserialized GameMaps</returns>
        public List<GameMap> LoadMaps(string dirPath = @".\gameMaps\")
        {
            //if no map files, return blank map
            if (!Directory.GetFiles(dirPath).Any(name => name.EndsWith(".wamap", StringComparison.InvariantCulture)))
            {
                return new List<GameMap>
                {
                    new GameMap()
                };
            }

            var serializer = new BinaryFormatter();

            var dir = Directory.GetFiles(dirPath);

            var maps = from file in dir
                       where file.EndsWith(".wamap", StringComparison.InvariantCulture)
                       select JsonSerializer.Deserialize<GameMap>(File.ReadAllText(file));

            return maps.ToList();
        }
    }
}