using CloneMessengerBackendAPI.Model.ConfigureModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.BaseModels
{
    public static class CacheMessage
    {
        public static CacheGroupModel GetCacheGroupMessage(Guid userId)
        {
            //Thời gian gửi tin nhắn trừ CachePreviousSendMessage (thời gian bắt đầu tính group message)
            //Nếu kq <= setting time cho 1 group message by time => keyGroupByTime xài cái key trong key
            //Nếu kq > setting time => keyGroupByTime + CachePreviousSendMessage set bằng value mới và xài key, time đó

            //Nếu get CacheByUser đúng trong thời gian tính group thì 
            // 1. CreateUserId same với cái latest CreatedUserId in cache thì lấy cacheKeyUser thì xài cái key đó
            // 2. CreateUserId not same với cái latest CreatedUserId in cache thì tạo value mới cacheKeyUser + xài cái key đó

            // Time : StartingGroupTime + KeyGroupByTime in Cache, SettingTime tính cho group
            // User : PreviousSendMessageUserId + KeyGroupByUser
            var keyGroupMessageCache = SettingKey.CacheGroupMessageKey;

            MemoryCache cache = MemoryCache.Default;
            var dateTime = DateTime.Now;
            var c = new CacheGroupModel()
            {
                StartingTime = dateTime,
                KeyGroupByTime = Guid.NewGuid(),
                KeyGroupByUserId = Guid.NewGuid(),
                PreviousSendMessageUserId = userId,
            };
            CacheItem newCache = new CacheItem(keyGroupMessageCache, c);

            //Nếu ko có keyCache => Tạo mới
            if (cache.Contains(keyGroupMessageCache) == false)
            {
                cache.Set(newCache, null);
            }
            else
            {
                //Nếu có keyCache
                var currentCache = (CacheGroupModel)cache.GetCacheItem(keyGroupMessageCache).Value;
                var subTime = dateTime.Subtract(currentCache.StartingTime);
                var settingTime = new TimeSpan(DefaultConfig.DefaultHourTMessageInGroupMessage, 0, 0);
                if (subTime <= settingTime)
                {
                    if (currentCache.PreviousSendMessageUserId != userId)
                    {
                        c.PreviousSendMessageUserId = userId;
                        c.KeyGroupByUserId = Guid.NewGuid();
                    }
                }
                else
                {
                    c.PreviousKeyGroupByTime = currentCache.KeyGroupByTime;
                    cache.Set(newCache, null);
                }
            }
            var myCache = (CacheGroupModel)cache.GetCacheItem(keyGroupMessageCache).Value;

            return new CacheGroupViewModel(myCache);
        }
    }

    public class CacheGroupViewModel : CacheGroupModel
    {
        public bool IsCreatedNewGroup => PreviousKeyGroupByTime != KeyGroupByTime;
        public CacheGroupViewModel() { }
        public CacheGroupViewModel(CacheGroupModel cache)
        {
            this.StartingTime = cache.StartingTime;
            this.KeyGroupByTime = cache.KeyGroupByTime;
            this.PreviousKeyGroupByTime = cache.PreviousKeyGroupByTime;
            this.PreviousSendMessageUserId = cache.PreviousSendMessageUserId;
            this.KeyGroupByUserId = cache.KeyGroupByUserId;
        }
    }

    public class CacheGroupModel
    {
        public DateTime StartingTime { get; set; }
        public Guid KeyGroupByTime { get; set; }
        public Guid? PreviousKeyGroupByTime { get; set; }
        public Guid PreviousSendMessageUserId { get; set; }
        public Guid KeyGroupByUserId { get; set; }
    }
}
