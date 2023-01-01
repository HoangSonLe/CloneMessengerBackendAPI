using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Model.ConfigureModel
{
    public class CacheGroupTime
    {
        public DateTime StartingTime { get; set; }
        public Guid KeyGroupByTime { get; set; }
    }
    public class CacheGroupUserMessage
    {
        public Guid PreviousSendMessageUserId { get; set; }
        public Guid KeyGroupByUserId { get; set; }
    }
    public class CacheGroup
    {
        public DateTime StartingTime { get; set; }
        public Guid KeyGroupByTime { get; set; }
        public Guid PreviousSendMessageUserId { get; set; }
        public Guid KeyGroupByUserId { get; set; }
        //public CacheGroupTime CacheGroupTime { get; set; }
        //public CacheGroupUserMessage CacheGroupUserMessage { get; set; }
    }
}
