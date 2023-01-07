using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Model.ConfigureModel
{
    public static class DefaultConfig
    {
        public static readonly int DefaultPageSize = 20;
        public static readonly int DefaultHourMessage = 60;
        public static readonly TimeSpan DefaultHourMessageInGroupMessage = new TimeSpan(DefaultHourMessage, 0, 0);
    }
    public class Config
    {
        public static Config Default
        {
            get
            {
                return new Config();
            }
        }
        public readonly DateTimeOffset ExpireTimeCache = new DateTimeOffset(DateTime.Now.AddHours(DefaultConfig.DefaultHourMessage * 2));
    }
    public static class SettingKey
    {
        //public static string CacheGroupMessageKey => "keyGroupMessageCache";
    }
}
