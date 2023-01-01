using CloneMessengerBackendAPI.Model.ConfigureModel;
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
            PageSize = DefaultConfig.DefaultPageSize;
        }
    }
    public class PaginationModel<T> : PaginationModel
    {
        public T Data { get; set; }
    }
    public class ChatMessagePaginationModel : PaginationModel
    {
        public Guid ChatGroupId { get; set; }
    }
}
