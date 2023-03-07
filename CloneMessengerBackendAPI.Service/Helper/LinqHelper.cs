using CloneMessengerBackendAPI.Service.Models.BaseModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Helper
{
    public static class LinqHelper
    {
        public static async Task TrySaveChangesAsync(this Acknowledgement ack, DbContext context, Action<Exception> handleError = null)
        {
            try
            {
                await context.SaveChangesAsync();
                ack.IsSuccess = true;
            }
            catch (Exception ex)
            {
                handleError?.Invoke(ex);
                ack.IsSuccess = false;
                ack.ExtractMessage(ex);
            }
        }
        public static List<T> ToSingleList<T>(this T data)
        {
            return new List<T> { data };
        }
    }
}
