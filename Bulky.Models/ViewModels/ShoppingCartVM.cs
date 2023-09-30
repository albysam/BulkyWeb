using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        public OrderHeader OrderHeader { get; set; }
        public Address Address { get; set; }
        //public IEnumerable<Address> AddressList { get; set; }
        public Coupon Coupon { get; set; }

		public string SelectedCouponCode { get; set; }
	}
}
