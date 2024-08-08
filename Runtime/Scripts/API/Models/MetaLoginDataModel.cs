using System;

namespace BRIJ.Code.Models
{
    [Serializable]
    public class MetaLoginDataModel
    {
        public string gameToken;
        public string oculusNonce;
        public ulong userId;
    }
}