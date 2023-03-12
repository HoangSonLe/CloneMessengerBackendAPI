using CloneMessengerBackendAPI.Service.Helper;
using CloneMessengerBackendAPI.Service.Interfaces;
using EHealth.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Web.Controllers
{
    public class FileController : BaseController
    {
        public FileController(IUserService userService, IMessageService messageService) : base(userService, messageService)
        {

        }
        public async Task<System.Web.Mvc.FileResult> GetFileData(Guid fileId)
        {
            var result = await MessageServices.GetFiles(fileId.ToSingleList());
            if (result.IsSuccess && result.Data.Count() == 1)
            {
                var file = result.Data[0];
                if (file != null)
                {
                    String fileName = file.Name;
                    if (String.IsNullOrEmpty(fileName) == true || fileName.Contains(file.Ext) == false)
                    {
                        fileName = "Default" + "." + file.Ext;
                    }
                }
                if (file.IsImage())
                {
                    return new System.Web.Mvc.FileContentResult(file.Data, "image.jpg");
                }
                return File(file.Data, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
            }
            return null;
        }
    }
}
