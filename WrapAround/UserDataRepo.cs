using System.Collections.Generic;

namespace WrapAround
{
    public class UserDataRepo : IUserGameRepository 
    {
        /// <summary>
        /// Keys are UserIds, Values are the lobby they are in
        /// </summary>
        public Dictionary<string, string> UserDictionary { get; set; } = new Dictionary<string, string>();

    }
}