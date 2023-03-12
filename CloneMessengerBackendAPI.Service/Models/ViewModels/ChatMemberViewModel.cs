using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatMemberViewModel
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
        public bool IsOnline { get; set; }
        public string AddByName { get; set; }
        public Guid? AvatarFileId { get; set; }
        public string AvatarFileSrc { get; set; }
    }
}
