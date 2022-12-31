using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class PageDefaultModel
    {
        public PaginationModel PaginationModel { get; set; }
        public ChatMessagePaginationModel ChatMessagePaginationModel { get; set; }
        public ChatMessagePostData MessagePostData { get; set; }
        public CreateChatGroupModel CreateChatGroupModel { get; set; }

        public PageDefaultModel()
        {
            PaginationModel = new PaginationModel();
            ChatMessagePaginationModel = new ChatMessagePaginationModel();
            MessagePostData= new ChatMessagePostData();
            CreateChatGroupModel= new CreateChatGroupModel();
        }
    }
}
