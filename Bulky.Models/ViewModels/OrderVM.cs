using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
	public class OrderVM
	{
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<OrderDetail> OrderDetail { get; set; }
		public WalletTotal? WalletTotal { get; set; }
		public Coupon? Coupon { get; set; }
	
		public Address Address { get; set; }
	}
}
