using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models
{
	public class LazyLoadPagination
	{
		public int Skip { get; set; }
		public int? PageSize { get; set; }
		public bool CanLoadMore { get; set; }
	}
}
