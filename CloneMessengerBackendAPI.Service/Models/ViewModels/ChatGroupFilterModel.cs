using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models
{
    public class ChatMessageFilterModel : LazyLoadPagination
    {
        public ChatMessageFilterModel()
        {
            Skip = 0;
            PageSize = 20;
        }
    }
}
