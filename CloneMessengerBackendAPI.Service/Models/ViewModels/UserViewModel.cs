using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

    }
}
