using System;

namespace BRIJ.Code.Models
{
    [Serializable]
    public class RestoreSessionModel
    {
        public bool successful;
        public UserModel user;
        
        public RestoreSessionModel() {}

        public RestoreSessionModel(bool successful, UserModel user)
        {
            this.successful = successful;
            this.user = user;
        }
    }
}