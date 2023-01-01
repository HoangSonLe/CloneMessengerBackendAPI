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
        public static readonly int DefaultHourTMessageInGroupMessage = 60;
    }
    public static class SettingKey
    {
        public static string CacheGroupMessageKey => "keyGroupMessageCache";
    }
}
