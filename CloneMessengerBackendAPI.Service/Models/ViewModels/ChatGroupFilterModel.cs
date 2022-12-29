using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models
{
    public class PaginationModel : LazyLoadPagination
    {
        public PaginationModel()
        {
            Skip = 0;
            PageSize = 20;
        }
    }
    public class ChatMessagePaginationModel : PaginationModel
    {
        public Guid ChatGroupId { get; set; }
    }
}
