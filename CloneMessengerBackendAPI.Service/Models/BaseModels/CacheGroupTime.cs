using CloneMessengerBackendAPI.Model.ConfigureModel;
using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.BaseModels
{
    public static class CacheMessage
    {
        public static CacheGroupModel GetOrCreateCacheGroup(CloneMessengerDbContext context,Guid chatGroupId, Guid userId)
        {
            MemoryCache cache = MemoryCache.Default;
            CacheGroupModel result = null;
            CacheItem cacheItem = null;
            cacheItem = cache.GetCacheItem(chatGroupId.ToString());
            if (cacheItem != null)
            {
                result = (CacheGroupModel)cacheItem.Value;
                result = ProcessingCacheGroupMessage(result,userId);
                cache.Remove(chatGroupId.ToString());
            }
            else
            {
                result = CreateCacheGroupModel(context, chatGroupId, userId);
            }
            cache.Set(chatGroupId.ToString(), result, Config.Default.ExpireTimeCache);

            var myCache = (CacheGroupModel)cache.GetCacheItem(chatGroupId.ToString()).Value;

            return myCache;
        }
        private static CacheGroupModel CreateCacheGroupModel(CloneMessengerDbContext context, Guid chatGroupId,Guid userId)
        {
            var g = context.ChatGroups.Where(i=> i.Id == chatGroupId)
                                            .Include(i=> i.LastChatMessage)
                                            .First();
            var dateTime = DateTime.Now;
            var c = new CacheGroupModel()
            {
                StartingTime = dateTime,
                PreviousKeyGroupByTime = null as Guid?,
                KeyGroupByTime = Guid.NewGuid(),
                PreviousSendMessageUserId = null as Guid?,
                KeyGroupByUserId = Guid.NewGuid(),
                CurrentSendMessageUserId = userId,
            };
            if (g.LastChatMessage != null)
            {
                var l = g.LastChatMessage;
                var date = context.ChatMessages.Where(i => i.ContinuityKeyByTime == l.ContinuityKeyByTime)
                                                     .OrderBy(i => i.CreatedDate).Select(i => i.CreatedDate)
                                                     .First();
                c.StartingTime = date;
                c.PreviousKeyGroupByTime = l.ContinuityKeyByTime;
                c.PreviousSendMessageUserId = l.CreatedBy;
            }
            return c;

        }
        private static CacheGroupModel ProcessingCacheGroupMessage(CacheGroupModel currentCache, Guid userId)
        {
            //Thời gian gửi tin nhắn trừ CachePreviousSendMessage (thời gian bắt đầu tính group message)
            //Nếu kq <= setting time cho 1 group message by time => keyGroupByTime xài cái key trong key
            //Nếu kq > setting time => keyGroupByTime + CachePreviousSendMessage set bằng value mới và xài key, time đó

            //Nếu get CacheByUser đúng trong thời gian tính group thì 
            // 1. CreateUserId same với cái latest CreatedUserId in cache thì lấy cacheKeyUser thì xài cái key đó
            // 2. CreateUserId not same với cái latest CreatedUserId in cache thì tạo value mới cacheKeyUser + xài cái key đó

            // Time : StartingGroupTime + KeyGroupByTime in Cache, SettingTime tính cho group
            // User : PreviousSendMessageUserId + KeyGroupByUser
                
                var dateTime = DateTime.Now;
                var subTime = dateTime.Subtract(currentCache.StartingTime);
                var settingTime = DefaultConfig.DefaultHourMessageInGroupMessage;

                // Gán userId lại cho cache mỗi lần sendMessage
                currentCache.PreviousSendMessageUserId = currentCache.CurrentSendMessageUserId;
                currentCache.CurrentSendMessageUserId = userId;
                // Gán previousKeyGroupTime lại cho cache mỗi lần sendMessage
                currentCache.PreviousKeyGroupByTime = currentCache.KeyGroupByTime;

                if (subTime <= settingTime) //Không tạo group time mới
                {
                    if (currentCache.PreviousSendMessageUserId != currentCache.CurrentSendMessageUserId)
                    {
                        //Tạo mới cho keyGroupUser
                        currentCache.KeyGroupByUserId = Guid.NewGuid();
                    }
                }
                else //Tạo mới group time
                {
                    //Tạo mới cho keyGroupTime và keyGroupUser
                    currentCache.KeyGroupByTime = Guid.NewGuid();
                    currentCache.KeyGroupByUserId = Guid.NewGuid();
                }

            return currentCache;
        }
    }


    public class CacheGroupModel
    {
        public DateTime StartingTime { get; set; }
        /// <summary>
        /// Key group by time của lần message trước
        /// </summary>
        public Guid? PreviousKeyGroupByTime { get; set; }
        /// <summary>
        /// Key group by time của lần message hiện tại
        /// </summary>
        public Guid KeyGroupByTime { get; set; }
        /// <summary>
        /// Key group by user của lần message hiện tại
        /// </summary>
        public Guid KeyGroupByUserId { get; set; }

        /// <summary>
        /// UserId tạo message của lần hiện tại
        /// </summary>
        public Guid CurrentSendMessageUserId { get; set; }
        /// <summary>
        /// UserId tạo message của lần send trước
        /// </summary>
        public Guid? PreviousSendMessageUserId { get; set; }
        /// <summary>
        /// Check có tạo mới group time của các messages
        /// </summary>
        public bool IsNewGroupByTime => PreviousKeyGroupByTime != KeyGroupByTime;
        /// <summary>
        /// Check có tạo mới group user của messages
        /// </summary>
        public bool IsNewGroupByUser => IsNewGroupByTime || PreviousSendMessageUserId != CurrentSendMessageUserId;

    }
}
