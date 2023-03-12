using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models
{
    public enum EChatGroupStatus
    {
        InGroup = 1,
        IsRemoved = 2,
    }
    public enum EMessageStatus
    {
        Undefine = 0,
        Sending = 1,
        Sent = 2,
        Read = 3
    }
    public enum EFileType
    {
        Undefine = 0, 
        Image = 1,
        Others = 2,
    }
}
