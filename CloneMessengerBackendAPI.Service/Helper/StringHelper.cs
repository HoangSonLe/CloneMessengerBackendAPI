using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Helper
{
    public static class StringHelper
    {
        public static byte[] ToMD5Byte(this string text)
        {
            MD5 md5 = MD5.Create();
            md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(text));
            byte[] result = md5.Hash;
            return result;
        }

        public static string CreateChatGroupName (List<User> users,Guid? currentUserId)
        {
            var predicate = PredicateBuilder.New<User>(true);
            if(currentUserId!= null)
            {
                //Check if a group => not remove current user name
                if(users.Where(i=> i.Id != currentUserId).Count() <= 1)
                {
                    predicate = predicate.And(i=> i.Id != currentUserId);
                }
            }
            var s = string.Join(",",users.Where(predicate).Select(i=> i.DisplayName).ToList());
            return s;
        }
        public static string CastMemberToUserIdForChat(List<Guid> ids)
        {
            return string.Join(",", ids.Select(i => i.ToString()).OrderBy(i => i));
        }
        public static string FormatDateTime(DateTime time)
        {
            return time.ToString("dd/MM/yyyy HH:mm");
        }
        public static string FormatDate(DateTime time)
        {
            return time.ToString("dd/MM/yyyy");
        }
    }
}
