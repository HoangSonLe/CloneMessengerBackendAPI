using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Model.Helper
{
    public static class FileHelper
    {
        public static FileAttachment ToEmptyData(this FileAttachment f)
        {
            f.Data = null;
            return f;
        }
    }
}
