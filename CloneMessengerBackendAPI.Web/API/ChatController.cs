using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Helper;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Service.Serviecs;
using CloneMessengerBackendAPI.Web.Hubs;
using EHealth.Web.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CloneMessengerBackendAPI.Web.API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class ChatController : BaseAPIController
    {
        public ChatController(IUserService userService, IMessageService messageService) : base(userService, messageService)
        {
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> Test()
        {
            var result = await MessageServices.Test();
            return MapToIHttpActionResult(result);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPageDefaultModel()
        {
            var result = new Acknowledgement<PageDefaultModel>()
            {
                IsSuccess = true,
                Data = new PageDefaultModel()
                {
                    ChatGroupDetailViewModel = new ChatGroupDetailViewModel()
                    {
                        IsTmp = true,
                    },
                    ChatGroupViewModel = new ChatGroupViewModel()
                    {
                        IsTmp = true,
                        Name = "New message"
                    }
                }
            };
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        // GET: ChatGroup
        public async Task<IHttpActionResult> GetChatGroupList(PaginationModel post)
        {
            var p = post == null ? new PaginationModel() : post;
            p.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.GetChatGroups(p);
            if (result.IsSuccess)
            {
                result.Data.Data.ForEach(i =>
                {
                    i.ListMembers.ForEach(j =>
                    {
                        j.AvatarFileSrc = APIHelper.GetFileUrl(j.AvatarFileId);
                    });
                });
            }
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> ReadLastMessage(Guid chatGroupId)
        {
            var result = await MessageServices.ReadLastMessage(chatGroupId, (Guid)CurrentUserId());
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetChatGroupDetail(ChatMessagePaginationModel post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.GetChatGroupDetail(post);
            if (result.IsSuccess)
            {
                result.Data.ListMembers.ForEach(i =>
                {
                    i.AvatarFileSrc = APIHelper.GetFileUrl(i.AvatarFileId);
                });
            }
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetMessageList(ChatMessagePaginationModel post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.GetMessageList(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> SendMessage(ChatMessagePostData post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.SendMessage(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetUserList(string searchValue)
        {
            var result = await MessageServices.GetUserList(searchValue, (Guid)CurrentUserId());
            if (result.IsSuccess)
            {
                result.Data.ForEach(i =>
                {
                    i.AvatarFileSrc = APIHelper.GetFileUrl(i.AvatarFileId);
                });
            }
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateChatGroup(CreateChatGroupModel post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.CreateChatGroup(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> SearchChatGroup(List<Guid> memberIds)
        {
            var result = await MessageServices.SearchChatGroup(memberIds, GetCurrentUserModel());
            return MapToIHttpActionResult(result);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> UploadImage()
        {
            return await UploadFiles(true, EFileType.Image);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFiles(bool isAnonymous = false, EFileType fileType = EFileType.Undefine)
        {
            var request = HttpContext.Current.Request;
            //var request = this.Request();
            var requestFiles = request.Files;
            if (requestFiles.Count == 0)
            {
                return Content(HttpStatusCode.BadRequest, "No files");
            }
            var files = new List<FileAttachment>();
            for (var i = 0; i < requestFiles.Count; i++)
            {
                var file = requestFiles[i];
                if (fileType == EFileType.Image && file.IsImage() == false)
                {
                    return Content(HttpStatusCode.BadRequest, "File type is not valid!");
                }

                byte[] data;

                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    data = ms.ToArray();
                }
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    //Or
                //    const int bufferSize = 4096;
                //    byte[] buffer = new byte[bufferSize];
                //    int count;
                //    while ((count = file.InputStream.Read(buffer, 0, buffer.Length)) != 0)
                //        ms.Write(buffer, 0, count);
                //    data = ms.ToArray();
                //    //Or
                //    int count1;
                //    while ((count1 = file.InputStream.Read(buffer, 0, buffer.Length)) != 0)
                //        ms.Write(buffer, 0, count1);
                //    file.InputStream.Read(buffer, 0, buffer.Length);
                //    data = ms.ToArray();
                //}

                var fileAttachment = new FileAttachment();
                fileAttachment.Id = Guid.NewGuid();
                fileAttachment.Name = Path.GetFileName(file.FileName).ToLower();
                fileAttachment.Ext = Path.GetExtension(file.FileName);
                fileAttachment.CreatedBy = isAnonymous == true ? null : CurrentUserId();
                fileAttachment.CreatedDate = DateTime.Now.ToUniversalTime();
                fileAttachment.Data = data;

                files.Add(fileAttachment);
            }
            var result = await MessageServices.InsertFiles(files);
            return MapToIHttpActionResult(result);
        }
        public async Task<IHttpActionResult> GetFiles(List<Guid> fileIds)
        {
            var result = await MessageServices.GetFiles(fileIds);
            if (result.IsSuccess)
            {
                result.Data.ForEach(i =>
                {
                    i.AddFileURL();
                });
            }
            return MapToIHttpActionResult(result);
        }


    }
}
