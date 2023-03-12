using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneMessengerBackendAPI.Model.Model;

namespace CloneMessengerBackendAPI.Service.Interfaces
{
    public interface IMessageService : IBasicService
    {
        #region Messages
        Task<Acknowledgement<PaginationModel<List<ChatGroupViewModel>>>> GetChatGroups(PaginationModel post);
        Task<Acknowledgement<ChatGroupDetailViewModel>> GetChatGroupDetail(ChatMessagePaginationModel post);
        Task<Acknowledgement> SendMessage(ChatMessagePostData post);
        Task<Acknowledgement<PaginationModel<List<ChatMessageGroupByTimeViewModel>>>> GetMessageList(ChatMessagePaginationModel post);
        Task<Acknowledgement<List<UserViewModel>>> GetUserList(string searchValue, Guid currentUserId);
        Task<Acknowledgement> CreateChatGroup(CreateChatGroupModel post);
        Task<Acknowledgement> ReadLastMessage(Guid chatGroupId, Guid currentUserId);
        Task<Acknowledgement<ChatGroupDetailViewModel>> SearchChatGroup(List<Guid> memberIds, UserModel currentUser);
        #endregion

        #region Files
        Task<Acknowledgement<List<FileAttachment>>> InsertFiles(List<FileAttachment> files);
        Task<Acknowledgement<List<FileAttachment>>> GetFiles(List<Guid> fileIds);
        #endregion
    }
}
