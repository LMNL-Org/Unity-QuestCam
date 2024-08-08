using System;

namespace BRIJ.Models
{
    [Serializable]
    public struct LoginModel
    {
        public string emailOrUsername;
        public string password;
        public string token;
    }
}