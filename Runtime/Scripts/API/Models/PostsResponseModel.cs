using System;
using System.Collections.Generic;
using BRIJ.Models;

namespace BRIJ.Code.Models
{
    [Serializable]
    public class PostsResponseModel
    {
        public string successful;
        public List<PostModel> posts;
    }
}