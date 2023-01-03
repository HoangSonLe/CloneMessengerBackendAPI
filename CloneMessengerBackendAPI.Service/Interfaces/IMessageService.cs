using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Interfaces
{
    public interface IMessageService : IBasicService
    {
        Task<Acknowledgement<PaginationModel<List<ChatGroupViewModel>>>> GetChatGroups(PaginationModel post);
        Task<Acknowledgement<ChatGroupDetailViewModel>> GetChatGroupDetail(ChatMessagePaginationModel post);
        Task<Acknowledgement> SendMessage(ChatMessagePostData post);
        Task<Acknowledgement<PaginationModel<List<ChatMessageGroupByTimeViewModel>>>> GetMessageList(ChatMessagePaginationModel post);
        Task<Acknowledgement<List<UserViewModel>>> GetUserList(string searchValue);
        Task<Acknowledgement> CreateChatGroup(CreateChatGroupModel post);


    }
}
