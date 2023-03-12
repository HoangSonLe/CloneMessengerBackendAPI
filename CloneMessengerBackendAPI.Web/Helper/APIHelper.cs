
using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace EHealth.Web.Helper
{
    public static class APIHelper
    {

        public static HttpContextWrapper GetContext(this ApiController controller)
        {
            return ((HttpContextWrapper)controller.Request.Properties["MS_HttpContext"]);
        }
        public static HttpRequestBase Request(this ApiController controller)
        {
            return controller.GetContext().Request;
        }
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
        public static bool IsImage(this HttpPostedFile f)
        {
            return ImageExtensions.Contains(Path.GetExtension(f.FileName).ToUpperInvariant());
        }
        public static bool IsImage(this FileAttachment f)
        {
            return ImageExtensions.Contains(f.Ext.ToUpperInvariant());
        }
        public static string ToAbsoluteUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl)) return relativeUrl;
            if (HttpContext.Current == null) return null;
            if (relativeUrl.StartsWith("/")) relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");
            var url = HttpContext.Current.Request.Url;
            var port = url.Port == 44344 ? (":" + url.Port) : String.Empty;
            return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));

        }
        public static string GetFileUrl(Guid? fileId)
        {
            if(fileId == null) return null;
            return ToAbsoluteUrl("/File/GetFileData?fileId=" + fileId.ToString());
        }
        public static void AddFileURL(this FileAttachment attachment)
        {
            //attachment.DownloadUrl = ToAbsoluteUrl("/File/GetFileData?fileIds=" + attachment.Id.ToString());
            //if (attachment.IsImage)
            //{
            //    attachment.ThumbnailUrl = ToAbsoluteUrl("/File/ThumbnailUrl?id=" + attachment.Id.ToString());
            //    attachment.ImageUrl = ToAbsoluteUrl("/File/ImageUrl?id=" + attachment.Id.ToString());
            //}
        }
    }
}