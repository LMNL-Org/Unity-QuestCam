using System;
using System.Collections.Generic;

namespace BRIJ.Models
{
    [Serializable]
    public class PostReactionModel : IComparable<PostReactionModel>
    {
        public string name;
        public int count;
        public bool reacted;

        public PostReactionModel()
        {
        }

        public PostReactionModel(string name, int count, bool reacted)
        {
            this.name = name;
            this.count = count;
            this.reacted = reacted;
        }

        public int CompareTo(PostReactionModel other)
        {
            return count.CompareTo(other.count);
        }
    }
    
    [Serializable]
    public class PostModel
    {
        public long id;
        public UserModel user;
        public string image;
        public string experienceName;
        public string experienceUrl;
        public string postTime;
        public List<PostReactionModel> reactions;
    }
}