using System;

namespace BRIJ.Code.Models
{
    [Serializable]
    public class GameSessionModel
    {
        public bool successful;
        public string sessionToken;
        public UserModel user;
    }
}