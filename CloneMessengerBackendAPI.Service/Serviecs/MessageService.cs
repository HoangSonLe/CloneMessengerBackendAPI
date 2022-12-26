using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public interface IMessageServices
    {
        Task<List<ChatGroup>> GetChatGroups();

    }
    public class MessageService : IMessageServices
    {
        private CloneMessengerDbContext DbContext = new CloneMessengerDbContext();
        public async Task<List<ChatGroup>> GetChatGroups()
        {
            var chatGroups = await DbContext.ChatGroups.ToListAsync();

            return chatGroups;
        }
    }
}
