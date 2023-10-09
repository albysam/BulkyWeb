using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
	public class AddressVM
	{
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<Address> Address { get; set; }
		
	}
}
