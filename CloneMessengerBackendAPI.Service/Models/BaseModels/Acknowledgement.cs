using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.BaseModels
{
    public class BaseModelWithUserIdentity
    {
        public UserModel CurrentUser { get; set; }
    }
    public class Acknowledgement
    {
        public bool IsSuccess { get; set; }

        public List<string> ErrorMessage { get; set; }

        public List<string> SuccessMessage { get; set; }

        public Acknowledgement()
        {
            IsSuccess = false;
            ErrorMessage = new List<string>();
            SuccessMessage = new List<string>();
        }

        public void AddMessage(string message)
        {
            ErrorMessage.Add(message);
        }

        public void AddMessages(params string[] messages)
        {
            ErrorMessage.AddRange(messages);
        }

        public void AddMessages(IEnumerable<string> messages)
        {
            ErrorMessage.AddRange(messages);
        }

        public void AddSuccessMessages(params string[] messages)
        {
            SuccessMessage.AddRange(messages);
        }

        public Exception ToException()
        {
            if (!IsSuccess)
            {
                return new Exception(string.Join(Environment.NewLine, ErrorMessage));
            }

            return null;
        }

        public void ExtractMessage(Exception ex)
        {
            AddMessage(ex.Message);
            if (ex.InnerException != null)
            {
                ExtractMessage(ex.InnerException);
            }
        }
    }
   
    public class Acknowledgement<T> : Acknowledgement
    {
        public T Data { get; set; }
    }
}
